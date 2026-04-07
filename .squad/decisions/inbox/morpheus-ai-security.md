# Decision: LLM Response Validation & Prompt Hardening (FINDING-003)

**Author:** Morpheus (SDK Dev)
**Date:** 2026-04-07
**Status:** Implemented
**Finding:** FINDING-003 — Prompt Injection via Malicious Source Files

## Context

Seraph's security audit identified that `SemanticExtractor` embeds file content verbatim into LLM prompts. A malicious source file can contain injected instructions that poison the LLM output, producing graph nodes with XSS payloads or false edges.

## Decision

Three-layer defense:

1. **Output validation** (`LlmResponseValidator.cs`): Validates JSON schema, sanitizes all labels via `InputValidator.SanitizeLabel()`, rejects script tags/HTML injection, drops orphaned edges, enforces max lengths, clamps weights.

2. **Prompt hardening** (`ExtractionPrompts.cs`): Added `===BEGIN/END===` delimiters around source content, anti-injection instruction, content truncation at 100K chars.

3. **Fail-safe wiring**: Both `SemanticExtractor` and `CopilotExtractor` route LLM responses through the validator. Invalid responses produce empty results — pipeline continues safely.

## Impact

- All LLM-generated graph data is sanitized before entering the pipeline
- Prompt injection attacks produce empty results instead of poisoned nodes/edges
- No behavior change for legitimate LLM responses
- Both extraction paths (core + SDK) are covered

## Team Notes

- Tank: Test data for `SemanticExtractorTests` updated — edges must reference existing nodes (validator enforces referential integrity)
- Trinity: Graph builder now receives pre-sanitized data — no additional sanitization needed at graph level
- Seraph: FINDING-003 resolved. Recommend re-audit to verify coverage
