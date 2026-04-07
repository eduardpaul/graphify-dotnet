# Decision: README Rewrite + Documentation Restructure

**Author:** Trinity (Core Developer)
**Date:** 2026-04-08
**Status:** Implemented

## Context

The README was 313 lines and growing. It duplicated content that belonged in docs/ — full CLI usage, configuration details, export format tables, worked examples. Bruno requested a short README focused on the 3-step quick start flow: install → `graphify config` → `graphify run`.

## Decision

1. **README shrunk from 313 → 71 lines.** Kept: badges, tagline, origin story, build-from-source, documentation link table, license, author, acknowledgments. Removed all detailed sections (features, architecture, CLI usage, configuration, export formats, worked example).

2. **Created 3 new docs:**
   - `docs/configuration.md` — full config system (wizard, layered priority, env vars, user secrets, `--config` flag)
   - `docs/cli-reference.md` — all 4 commands with every option documented
   - `docs/worked-example.md` — samples/mini-library walkthrough

3. **No content duplication.** README links to docs/ for everything beyond quick start.

4. **`graphify config` is step 2** in the README Quick Start, before `graphify run`. This is the primary onboarding flow.

## Alternatives Considered

- Keep a medium-length README (~150 lines) with abbreviated sections. Rejected — still too much for a front page. The link table approach is cleaner.

## Impact

- New users see install/configure/run in 30 seconds
- Detailed docs are discoverable via the link table
- No existing doc links broken
- All existing docs/ files unchanged
