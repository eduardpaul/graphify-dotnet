# Documentation Fixes: SDK API + Config Priority + JSON Schema

**Author:** Morpheus (SDK Dev)
**Date:** 2026-04-07
**Status:** Applied (pending commit)

## Context

Tank's validation report identified critical documentation errors across 8 files. All errors were in SDK-related content — wrong API methods, wrong configuration priority, and a fabricated JSON schema.

## Changes Made

### 1. M.E.AI API Migration (setup-ollama.md, setup-azure-openai.md)
- `client.CompleteAsync(prompt)` → `client.GetResponseAsync([new ChatMessage(ChatRole.User, prompt)])`
- `response.Message` → `response.Text`
- Verified against SemanticExtractor.cs and CopilotChatClient.cs

### 2. Configuration Priority (4 docs)
- Swapped env vars and user secrets to match ConfigurationFactory.cs
- Correct order: CLI args > user secrets > env vars > appsettings.local.json > appsettings.json
- Added missing appsettings.local.json layer to 3 setup docs

### 3. JSON Schema (format-json.md)
- Removed fictitious top-level `communities` array
- Removed non-existent node fields: `degree`, `description`
- Removed non-existent edge field: `extractionMethod`
- Fixed metadata fields to snake_case matching JsonExporter.cs DTOs
- Removed non-existent `version` and `source` metadata fields

### 4. Other Fixes
- worked-example.md: 35 → 30 obsidian files
- watch-mode.md: namespace, constructor params, default format
- dotnet-tool-install.md: added `-p` short alias
- GitHub URLs: BrunoCapuano → elbruno in 2 docs

## Impact

All documentation now matches actual source code. No code changes were needed — docs-only fixes.
