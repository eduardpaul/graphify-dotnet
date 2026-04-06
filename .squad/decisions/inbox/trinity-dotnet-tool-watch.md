# Decision: dotnet tool Packaging + Watch Mode Architecture

**Author:** Trinity (Core Developer)
**Date:** 2026-04-07
**Status:** Implemented

## Context

Two features needed for the short-term roadmap:
1. Make the CLI installable as `dotnet tool install --global graphify-dotnet`
2. Add incremental watch mode that monitors files and re-processes only changes

## Decision

### Feature 1: dotnet tool packaging
Added full NuGet tool metadata to `Graphify.Cli.csproj`:
- `PackAsTool=true`, `ToolCommandName=graphify`, `PackageId=graphify-dotnet`
- Version 0.1.0, MIT license, Bruno Capuano as author
- README.md included as package readme

### Feature 4: Watch mode architecture
- `WatchMode` class lives in **core library** (`Graphify.Pipeline`), not CLI
- Accepts a pre-built `KnowledgeGraph` via `SetInitialGraph()` — the initial pipeline run is caller's responsibility
- This avoids circular dependency (CLI references Core, not vice versa) while keeping WatchMode reusable by SDK/MCP
- Uses `FileSystemWatcher` + 500ms debounce + SHA256 content check
- Incremental: extract changed → merge into graph → re-cluster → re-export

## Impact

- CLI is now packageable as a global dotnet tool
- Watch mode enables dev-loop usage (edit code → see graph update)
- WatchMode is reusable outside CLI (e.g., MCP server could use it)
