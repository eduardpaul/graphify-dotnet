# Decision: CopilotExtractor for GitHub Copilot SDK Integration

**Author:** Morpheus (SDK/Integration Developer)  
**Date:** 2026-04-06  
**Status:** Implemented (with dependency gap)

## Context

The Graphify.Sdk project needed an extractor specifically designed for GitHub Copilot and GitHub Models API integration. While SemanticExtractor provides a generic Microsoft.Extensions.AI-based approach, users wanted a dedicated option for GitHub's AI backends.

## Problem

Two paths were available:
1. **GitHub.Copilot.SDK** NuGet package — Official SDK with session-based API, tool calling, multi-turn conversations
2. **IChatClient with GitHub Models API** — Use Microsoft.Extensions.AI with GitHub Models endpoint

The GitHub.Copilot.SDK requires:
- Local GitHub Copilot CLI process running
- JSON-RPC communication between SDK and CLI
- Session management and lifecycle handling
- More complex setup for simple extraction tasks

For graph extraction use cases (stateless, single-turn LLM calls to extract nodes/edges), this added complexity provides no value.

## Decision

Implemented **CopilotExtractor using Microsoft.Extensions.AI's IChatClient configured for GitHub Models API endpoint** (`https://models.inference.ai.azure.com`).

### Implementation Details

1. **CopilotExtractor.cs**: Mirrors SemanticExtractor architecture
   - Implements `IPipelineStage<DetectedFile, ExtractionResult>`
   - Reuses `ExtractionPrompts` from core library
   - Graceful degradation and error handling
   - Same JSON parsing and conversion logic

2. **CopilotExtractorOptions.cs**: Configuration for GitHub Models
   - `ApiKey` (GitHub PAT or token)
   - `ModelId` (e.g., "gpt-4o", "claude-3.5-sonnet")
   - `Endpoint` (defaults to GitHub Models API)
   - Standard extraction options (Temperature, MaxTokens, file size limits, category toggles)

3. **GitHubModelsClientFactory.cs**: Creates IChatClient for GitHub Models
   - Currently stubbed (requires `Microsoft.Extensions.AI.OpenAI` package)
   - Future: Implement OpenAI-compatible client configuration
   - Pattern: `new OpenAIChatClient(options, model).AsChatClient()`

### Why Not GitHub.Copilot.SDK

- **Overkill for extraction:** SDK designed for conversational agents, tool calling, multi-turn interactions
- **CLI dependency:** Requires GitHub Copilot CLI running locally (authentication, session management)
- **JSON-RPC complexity:** Adds extra layer of communication for simple HTTP API calls
- **Consistency:** IChatClient approach aligns with existing SemanticExtractor pattern

### Value Proposition

- **Choice:** Users can pick generic (SemanticExtractor) or GitHub-specific (CopilotExtractor) backends
- **Free tier:** GitHub Models offers free access to GPT-4o, Claude 3.5 Sonnet, and other models
- **Simple auth:** Just a GitHub token, no CLI setup required
- **Swappable:** Both implement same pipeline interface

## Open Items

1. **Add Microsoft.Extensions.AI.OpenAI package** to Graphify.Sdk.csproj
2. **Implement GitHubModelsClientFactory.Create()** with OpenAIChatClient configuration
3. **Document GitHub token acquisition** in README or user guide
4. **Test with actual GitHub Models API** (requires GitHub account with model access)

## Alternatives Considered

1. **Use GitHub.Copilot.SDK directly**: Rejected due to CLI dependency and session complexity
2. **Extend SemanticExtractor with GitHub Models support**: Rejected to keep clean separation of concerns
3. **Wait for Microsoft.Extensions.AI to add GitHub Models provider**: Too slow, IChatClient already supports OpenAI-compatible endpoints

## Impact

- **Minimal code duplication:** Most logic mirrors SemanticExtractor (intentional, reduces risk)
- **Dependency gap:** Requires Microsoft.Extensions.AI.OpenAI package (not yet added)
- **User experience:** Consistent interface, easy to swap between extractors
- **Future-proof:** If GitHub.Copilot.SDK improves, we can add a third extractor without changing pipeline

## Related

- SemanticExtractor implementation (Morpheus, 2026-04-06)
- ExtractionPrompts design (Trinity, 2026-04-06)
- Microsoft.Extensions.AI abstraction decision (Neo, 2026-04-06)
