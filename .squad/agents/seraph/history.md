# Seraph — History

## Project Context
- **Project:** graphify-dotnet — .NET 10 AI-powered knowledge graph builder for codebases
- **Owner:** Bruno Capuano
- **Stack:** .NET 10, C#, GitHub Copilot SDK, Microsoft.Extensions.AI, TreeSitter, QuikGraph, System.CommandLine, Spectre.Console, xUnit
- **Repo:** https://github.com/elbruno/graphify-dotnet
- **Key areas:** CLI tool (dotnet tool), config system (appsettings + user secrets + env vars), AI providers (Ollama, Azure OpenAI, Copilot SDK), file system scanning, AST parsing, graph export (7 formats)

## Learnings
- Joined the team for security review of v0.5.0 release

### 2026-04-07: Full Security Audit Completed
- **Report:** `.squad/decisions/inbox/seraph-security-audit.md` — 14 findings (2 High, 6 Medium, 4 Low, 2 Info)
- **Dependency scan:** `dotnet list graphify-dotnet.slnx package --vulnerable` — zero known CVEs
- **Top priority:** FINDING-002 (UnsafeRelaxedJsonEscaping in HtmlExporter.cs:17) — direct XSS vector, single-line fix
- **Second priority:** FINDING-001 (API keys in plaintext appsettings.local.json) — ConfigPersistence.cs should use user-secrets only
- **Key architecture insight:** `InputValidator` exists with SSRF/traversal/XSS protection but is NOT called by the pipeline (FileDetector, SemanticExtractor, PipelineRunner bypass it). Wiring it in would fix FINDING-005, 006, 012 simultaneously.
- **AI security surface:** LLM responses from SemanticExtractor flow unsanitized into all 7 export formats. Prompt injection via malicious source files is the primary attack vector (FINDING-003).
- **CI/CD:** publish.yml has expression injection risk (FINDING-007); also contradicts "not a NuGet package" conventions (FINDING-013) since Graphify.Cli.csproj has PackAsTool=true.
- **Positive patterns:** Nullable refs enabled, code analysis on, CancellationToken propagation, SVG escaping correct, user-secrets mechanism available, no unsafe code.
- **Floating versions (e.g., `10.*`, `0.*`)** in all .csproj files — supply chain risk if a dependency is compromised. Recommend pinning for release builds.
