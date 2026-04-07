# Decision: Documentation Overhaul — Neo's Improvement Plan

**Author:** Trinity (Core Developer)  
**Date:** 2026-04-07  
**Status:** Implemented

## Context

Neo audited all 19 docs + README as a new user and produced a 13-item improvement plan (`neo-docs-improvement-plan.md`). Implemented the top 8 priority items covering the critical gap: no guided tutorial, shallow worked example, scattered troubleshooting, and inconsistencies across docs.

## What Changed

### New Files
- `docs/getting-started.md` — Real step-by-step tutorial (install → first analysis → interpret results → add AI → try your own code)
- `docs/troubleshooting.md` — Central FAQ with 10 common problems
- `ROADMAP.md` — Moved from `docs/future-plans.md` (OSS convention)
- `.squad/image-prompts.md` — Moved from `docs/` (not user-facing)

### Rewritten
- `docs/worked-example.md` — Expanded from 48 to ~250 lines with real data walkthrough

### Fixed
- Default formats: Normalized to `json,html,report` everywhere (was inconsistent between docs)
- Blog post: Fixed non-existent CLI commands (`query`, `explain`, `export`) and format name (`GraphML`)
- Obsidian: Removed `--filter "community:Auth"` (flag doesn't exist)
- Ollama: Fixed user-facing code to use `AiProviderOptions` + `ChatClientFactory` instead of internal `OllamaOptions`
- dotnet-tool-install: Added `copilotsdk` to provider list
- Cross-links: All 7 format-*.md files now link to worked example
- README: Added Supported Languages table, AST-only note, Getting Started + Troubleshooting in docs table

## Key Decision

All user-facing code examples use `AiProviderOptions` + `ChatClientFactory.Create()` as the unified API. The internal `OllamaOptions`/`OllamaClientFactory` classes exist but aren't the recommended entry point for docs.

## Impact

A new user can now: install → run getting-started tutorial → understand output → explore worked example → troubleshoot problems — all without reading every doc. The docs table in README puts Getting Started first.
