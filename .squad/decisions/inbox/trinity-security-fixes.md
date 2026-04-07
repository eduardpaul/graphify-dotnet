### 2026-04-07: Security Hardening Implementation — Trinity

**Decision**: Implemented Seraph's security audit remediation (14 findings) across 3 phases on `squad/security-hardening`.

**Key Design Decisions**:

1. **API Key Storage (FINDING-001)**: API keys are now stored via `dotnet user-secrets` instead of `appsettings.local.json`. The `StoreApiKeyInUserSecrets()` method shells out to `dotnet user-secrets set` rather than using the Microsoft.Extensions.Configuration.UserSecrets API directly. This is simpler but has a subprocess dependency. The UserSecretsId is hardcoded — if it changes in the csproj, it must be updated in `ConfigPersistence.cs`.

2. **Output Path Validation (FINDING-006)**: `InputValidator.ValidatePath()` is called WITHOUT a base directory constraint. This validates for path traversal patterns (`..`, null bytes) but allows absolute paths anywhere. Using `Environment.CurrentDirectory` as base was too restrictive — it broke integration tests using temp dirs and would break any legitimate `--output /some/absolute/path` usage.

3. **HTML Template DOM Security (FINDING-004)**: Completely rewrote `showInfo()` and legend builder to use DOM API (`createElement`, `textContent`) instead of `innerHTML`. This is a larger change than minimal but eliminates the entire class of DOM XSS bugs rather than playing whack-a-mole with individual values.

4. **Convention Update (FINDING-013)**: Updated `.github/copilot-instructions.md` to acknowledge NuGet tool publishing. The old "this is NOT a NuGet package" language was misleading given the existing `publish.yml` and `PackAsTool` in the csproj. This change should propagate to any Copilot convention enforcement.

**Test Impact**: 598/599 tests pass. 1 pre-existing failure due to user-secrets on build machine.

**Dependencies on Other Team Members**:
- **Morpheus**: FINDING-003 (prompt injection) is in Morpheus's domain — not implemented here.
- **Tank**: Updated 2 test files to match new security behavior. Tank should review test assertions for the Cypher escaping tests — the substring-based `DoesNotContain` pattern doesn't work well for escape verification.

**Files Changed (15 total)**:
- `src/Graphify/Export/HtmlExporter.cs` — JavaScriptEncoder.Default
- `src/Graphify/Export/HtmlTemplate.cs` — DOM API for showInfo/legend
- `src/Graphify/Export/Neo4jExporter.cs` — Single-quote escaping
- `src/Graphify.Cli/Configuration/ConfigPersistence.cs` — API key to user-secrets
- `src/Graphify.Cli/PipelineRunner.cs` — Output path validation
- `src/Graphify.Cli/Program.cs` — Error sanitization, privacy warning
- `src/Graphify/Pipeline/FileDetector.cs` — Symlink detection, path validation
- `src/Graphify/Ingest/UrlIngester.cs` — Centralized SSRF validation
- `src/Graphify/Cache/SemanticCache.cs` — Unix file permissions
- `.github/workflows/publish.yml` — Expression injection fix
- `.github/copilot-instructions.md` — Convention alignment
- `Directory.Build.props` — Version pinning documentation
- `src/tests/Graphify.Tests/Cli/ConfigPersistenceTests.cs` — Updated for API key removal
- `src/tests/Graphify.Integration.Tests/Security/SecurityIntegrationTests.cs` — Updated Cypher assertion
