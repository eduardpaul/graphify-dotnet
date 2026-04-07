# Decision: NuGet Publish Workflow

**Author:** Trinity (Core Dev)  
**Date:** 2026-07-14  
**Status:** Implemented

## Context

The project was previously marked as "not a NuGet package" with "no publish workflow" (see original copilot-instructions decision). Bruno requested we prepare for NuGet publishing with v0.5.0, overriding that earlier decision.

## Decision

- Created `.github/workflows/publish.yml` triggered on GitHub release creation
- Version is baked into the csproj `<Version>` property — not derived from git tags
- Uses `secrets.NUGET_API_KEY` for authentication (simpler than OIDC for single-package repos)
- Symbol packages (.snupkg) included for debugging support
- `--skip-duplicate` makes the push idempotent (safe to re-run)

## Impact

- graphify-dotnet is now publishable to NuGet.org by creating a GitHub release
- The `NUGET_API_KEY` secret must be configured in the repo settings before first publish
- Version bumps are manual (edit csproj) — no tag-derived versioning
