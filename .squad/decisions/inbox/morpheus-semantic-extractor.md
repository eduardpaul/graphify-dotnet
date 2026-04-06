# Decision: SemanticExtractor Implementation with Microsoft.Extensions.AI

**Author:** Morpheus (SDK/Integration Developer)  
**Date:** 2026-04-06  
**Status:** Implemented  
**Related Commit:** 99c6cf6

## Context

The graphify-dotnet project needed semantic extraction to go beyond structural AST analysis and extract high-level concepts, design patterns, architectural relationships, and design rationale from code, documentation, and media files. This requires LLM integration but must remain model-agnostic.

The Python source (safishamsi/graphify) uses tree-sitter for structural extraction only. The README mentions semantic extraction for docs/images/papers, but the implementation was not visible in the Python codebase. The .NET port extends this with full LLM-based semantic extraction.

## Decision

Implemented `SemanticExtractor` as an `IPipelineStage<DetectedFile, ExtractionResult>` using **Microsoft.Extensions.AI** abstractions:

### Architecture

1. **Model Abstraction:** Uses `IChatClient` interface — no dependency on OpenAI, Anthropic, or any specific provider
2. **API:** `GetResponseAsync()` method (v10+ API; older `CompleteAsync` is deprecated)
3. **Response Type:** `ChatResponse` with direct `.Text` property
4. **Graceful Degradation:** If `IChatClient` is null, returns empty `ExtractionResult` — pipeline continues
5. **Error Handling:** All exceptions (API failures, malformed JSON, rate limits) caught and return empty result

### Prompt Design

Created `ExtractionPrompts` static class with four prompt templates:

1. **CodeSemanticExtraction:** Design patterns, architectural concepts, cross-cutting concerns, semantic similarities
2. **DocumentationExtraction:** Key concepts, entities, relationships, design rationale
3. **ImageVisionExtraction:** Diagrams, flowcharts, screenshots, whiteboards (any language) — requires vision model
4. **PaperExtraction:** Academic contributions, methods, citations, relationships

All prompts request structured JSON output:
```json
{
  "nodes": [{"id": "...", "label": "...", "type": "...", "metadata": {...}}],
  "edges": [{"source": "...", "target": "...", "relation": "...", "confidence": "...", "weight": 0.9}]
}
```

### Configuration

`SemanticExtractorOptions` controls:
- `ModelId` (optional override)
- `MaxTokens` (default: 4096)
- `Temperature` (default: 0.1 for structured output)
- Feature flags: `ExtractFromCode`, `ExtractFromDocs`, `ExtractFromMedia`
- `MaxNodesPerFile` (default: 15)
- `MaxFileSizeBytes` (default: 1MB)

### Confidence Tagging

All semantic edges are tagged `Confidence.Inferred` by default since they represent LLM interpretations, not directly extracted AST facts. The prompt can override to `EXTRACTED` for explicit statements or `AMBIGUOUS` for uncertain relationships.

## Alternatives Considered

1. **Direct OpenAI/Anthropic SDK:** Rejected — locks project to specific provider, violates model-agnostic principle
2. **No Semantic Extraction:** Rejected — limits graph to structural AST only, misses design patterns and rationale
3. **Tree-sitter Only (Python approach):** Rejected — doesn't extract concepts from docs/images/papers
4. **Embeddings + Vector DB:** Not needed yet — semantic edges from LLM prompts provide relationship signals directly

## Impact

- **Extensibility:** Any IChatClient implementation works (OpenAI, Azure OpenAI, Anthropic, local models, GitHub Copilot)
- **Robustness:** Pipeline never crashes due to LLM failures — graceful degradation everywhere
- **Token Efficiency:** Low temperature (0.1), structured JSON, error recovery minimize wasted tokens
- **Multimodal:** Supports code, docs, images, PDFs with appropriate prompts per category
- **Design Capture:** Extracts "why" (rationale) nodes, not just "what" (structure)

## Next Steps

1. **Integration:** Wire SemanticExtractor into the main pipeline after AST extraction
2. **Testing:** Unit tests with mock IChatClient to validate prompt formatting and JSON parsing
3. **Provider Integration:** Add GitHub Copilot SDK IChatClient adapter for production use
4. **PDF Text Extraction:** Integrate a PDF library for PaperExtraction (currently uses raw text)
5. **Vision Models:** Test with vision-capable models for ImageVisionExtraction

## Notes

- The Python source doesn't expose semantic extraction implementation — this is a .NET enhancement
- Microsoft.Extensions.AI v10+ renamed `CompleteAsync` → `GetResponseAsync`, `ChatCompletion` → `ChatResponse`
- JSON parsing includes fallback to extract from markdown code blocks (LLMs often wrap JSON in ```json)
- Node IDs use lowercase_with_underscores convention for consistency with Python graphify output
