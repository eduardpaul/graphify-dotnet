# Project Context

- **Owner:** Bruno Capuano
- **Project:** graphify-dotnet — A .NET 10 port of safishamsi/graphify, a Python AI coding assistant skill that reads files, builds a knowledge graph, and surfaces hidden structure. Uses GitHub Copilot SDK and Microsoft.Extensions.AI for semantic extraction.
- **Stack:** .NET 10, C#, GitHub Copilot SDK, Microsoft.Extensions.AI, Roslyn (AST parsing), xUnit, NuGet
- **Source:** https://github.com/safishamsi/graphify — Python pipeline: detect → extract → build_graph → cluster → analyze → report → export. Uses NetworkX, tree-sitter, Leiden community detection, vis.js.
- **Created:** 2026-04-06

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-04-06: SemanticExtractor Implementation

**What:** Implemented SemanticExtractor using Microsoft.Extensions.AI (IChatClient) for LLM-based semantic extraction of concepts and relationships from code, docs, and media files.

**Key Decisions:**
- Used `IChatClient` interface from Microsoft.Extensions.AI for model-agnostic LLM calls (no dependency on specific providers)
- API changed from `CompleteAsync` to `GetResponseAsync` in recent versions (v10+)
- Response type is `ChatResponse` with direct `.Text` property (not `.Message.Text`)
- Designed prompts to return structured JSON: `{ "nodes": [...], "edges": [...] }`
- All semantic relationships tagged as INFERRED by default (since they're interpretations, not direct AST)
- Graceful degradation: if IChatClient is null or errors occur, return empty ExtractionResult (pipeline continues)
- Error handling wraps API failures, malformed JSON, rate limits — never crashes pipeline

**Prompt Strategy:**
- **Code:** Extract design patterns, architectural concepts, cross-cutting concerns, semantic similarities
- **Docs:** Extract concepts, entities, relationships, design rationale
- **Images:** Vision-capable prompts for diagrams, flowcharts, screenshots, whiteboards (any language)
- **Papers:** Extract contributions, methods, citations, relationships

**Why Not Python Semantic Extraction:**
The Python source (safishamsi/graphify) uses tree-sitter for structural extraction only. The .NET port adds LLM-based semantic extraction as an enhancement, enabling deeper concept extraction beyond AST.

