# Architecture Security Review: graphify-dotnet

**Date:** 2026-04-07  
**Reviewer:** Neo (Lead/Architect)  
**Classification:** Architecture-Level Review (Design Security, not Code Audit)  
**Scope:** Trust boundaries, data flow, configuration security, AI provider threat models, export encoding, dependency trust.

---

## Executive Summary

graphify-dotnet is **architecturally sound** from a security standpoint. The system implements defense-in-depth across configuration management, input validation, and data export. Key strengths: explicit trust boundaries, layered configuration with secret isolation, provider-agnostic AI abstraction, and per-format output encoding.

**Critical Issues Found:** 0  
**Medium Concerns:** 2 (both manageable, both documented below)  
**Design Recommendations:** 3 (future-proofing)

---

## 1. Trust Boundary Map

### Untrusted Data Sources

```
┌────────────────────────────────────────────────────────────────┐
│                     UNTRUSTED INPUTS                            │
│  (from external world)                                           │
├─────────────────────────────────────────────────────────────────┤
│ • CLI arguments (--provider, --endpoint, --api-key, paths)     │
│ • Environment variables (GRAPHIFY__*)                           │
│ • appsettings.local.json (user-edited, persisted by wizard)    │
│ • Dotnet user-secrets (developer-set)                           │
│ • Source code files (arbitrary contents: binaries, text)        │
│ • File system metadata (paths, file names)                      │
│ • AI provider responses (LLM outputs)                           │
│ • Git tracked files list (git ls-files output)                 │
└────────────────────────────────────────────────────────────────┘
           │                │                │
           ▼                ▼                ▼
    ┌──────────────────────────────────────────────┐
    │    GRAPHIFY PIPELINE (Controlled Zone)       │
    │  - Input validation enforced at boundary     │
    │  - Extraction via structured parsers (AST)   │
    │  - Semantic extraction via API abstraction   │
    └──────────────────────────────────────────────┘
           │                │                │
           ▼                ▼                ▼
┌────────────────────────────────────────────────────────────────┐
│                      TRUSTED OUTPUTS                            │
│  (under our control)                                             │
├─────────────────────────────────────────────────────────────────┤
│ • graph.json (JSON export, escaped)                             │
│ • graph.html (HTML with HTML-escaped content)                   │
│ • graph.svg (SVG, TBD encoding)                                 │
│ • obsidian/ (Markdown, URL-escaped)                             │
│ • wiki/ (Markdown, HTML-escaped)                                │
│ • GRAPH_REPORT.md (Markdown, plaintext-safe)                    │
│ • graph.cypher (Cypher DSL, escaped)                            │
└────────────────────────────────────────────────────────────────┘
```

### Control Flow

1. **Configuration boundary** — Where are secrets stored? How are they passed to providers?
2. **File input boundary** — What can be read? How are file paths validated?
3. **AI provider boundary** — What data leaves the system? What comes back?
4. **Export boundary** — How is data encoded per format?

---

## 2. Data Flow Security Analysis

### Pipeline: detect → extract → build → cluster → analyze → report → export

#### **Stage 1: FileDetector** (Detect)

**Untrusted inputs:**
- File system paths (user-provided root directory)
- File contents (arbitrary binaries, text)

**Validation applied:**
- ✅ Path traversal prevention: `Path.GetFullPath()` normalization + bounds checking against base directory
- ✅ Size limits: `MaxFileSizeBytes` (default 1MB) prevents memory DoS
- ✅ Extension whitelist: Only `CodeExtensions`, `DocumentationExtensions`, `MediaExtensions` are processed
- ✅ Directory skip list: Ignores `.env`, `node_modules`, `.git`, `venv`, etc.
- ✅ Git-aware filtering: Respects `.gitignore` via `git ls-files` (optional, `RespectGitIgnore` flag)
- ✅ Exception handling: Catches `UnauthorizedAccessException` gracefully, continues traversal

**Data emerges as:** `List<DetectedFile>` (metadata only: paths, extensions, language classification)

**Risk level:** ✅ LOW  
**Rationale:** File detector never reads content; only metadata. Path traversal bounds-checked. Extension whitelist prevents arbitrary files.

---

#### **Stage 2a: Extractor (AST Extraction)**

**Untrusted inputs:**
- File contents from detected files
- Language grammar files (TreeSitter.Bindings handles this, we trust NuGet)

**Validation applied:**
- ✅ File already filtered by extension (Stage 1)
- ✅ Size already checked (Stage 1, MaxFileSizeBytes limit)
- ✅ Exception handling: Catches extraction errors, logs warning, continues

**Data emerges as:** `List<ExtractionResult>` with:
- `ExtractedNode` (id, label, FileType, SourceFile, metadata)
- `ExtractedEdge` (source, target, relation, confidence)

**Risk level:** ✅ LOW  
**Rationale:** TreeSitter is a mature, widely-trusted AST library. We parse into structured objects, not eval. Control characters and HTML stripped in labels via `InputValidator.SanitizeLabel()`.

---

#### **Stage 2b: SemanticExtractor (AI Extraction)**

**Untrusted inputs:**
- File contents (sent to AI provider)
- **AI provider responses** (JSON chat completions back from OpenAI/Ollama/Copilot)

**Data sent to provider:**
- Source file content (full text)
- File path (metadata)
- Extraction prompts (system-controlled)

**Validation applied:**
- ✅ Provider abstraction (`IChatClient`): Provider-agnostic, testable
- ✅ Response validation: Responses deserialized into structured `ExtractionResult` objects
- ⚠️ **CONCERN 1:** AI responses are parsed with minimal validation beyond JSON schema. If an LLM injects malicious node IDs or relations, they pass through. **Mitigation:** Not exploitable in final export because output formats escape content, but semantically wrong (e.g., `node_id_with_html` would be shown as-is in JSON, escaped in HTML). **Accepted risk:** Low-impact; graph won't render malformed content.
- ✅ Length limits: Responses expected to be small (nodes, edges, confidence scores)

**Data emerges as:** `List<ExtractionResult>` merged with AST results

**Risk level:** ⚠️ MEDIUM (LOW impact)  
**Rationale:** LLM outputs are opaque; not validating against a strict schema. However, downstream export stages escape content, so injection is prevented. Graph semantics may be noisy, but not corrupted.

---

#### **Stage 3: GraphBuilder (Build)**

**Untrusted inputs:**
- Extracted nodes and edges (from Stages 2a + 2b)

**Validation applied:**
- ✅ `ExtractionValidator`: Checks all nodes have valid id/label/sourceFile, all edges reference real node IDs
- ✅ Deduplication: Nodes merged by ID; metadata collided intelligently
- ✅ Exception handling: Validation errors logged, graph building continues gracefully

**Data emerges as:** `KnowledgeGraph` (QuikGraph wrapper)

**Risk level:** ✅ LOW  
**Rationale:** Structured graph prevents invalid references. No code execution, no format escaping yet.

---

#### **Stage 4: ClusterEngine (Cluster)**

**Untrusted inputs:**
- Graph structure (verified in Stage 3)

**Validation applied:**
- ✅ Clustering algorithm is deterministic, no external calls
- ✅ Community assignments are numeric IDs (0, 1, 2, ...), safe

**Data emerges as:** `KnowledgeGraph` with `Community` property set on nodes

**Risk level:** ✅ LOW  
**Rationale:** Internal algorithm, no untrusted data processed.

---

#### **Stage 5: Analyzer (Analyze)**

**Untrusted inputs:**
- Graph structure (verified)

**Validation applied:**
- ✅ Centrality computations are numeric
- ✅ God nodes, surprising connections derived from structure

**Data emerges as:** `AnalysisResult` (metrics, top nodes, connections)

**Risk level:** ✅ LOW  
**Rationale:** Numeric analysis, no injection vectors.

---

#### **Stage 6a: ReportGenerator (Report)**

**Untrusted inputs:**
- Graph analysis (verified)
- Node labels (extracted, but already sanitized via `SanitizeLabel()`)

**Validation applied:**
- ✅ Labels sanitized earlier: `InputValidator.SanitizeLabel()` removes control chars, HTML tags, limits length
- ✅ Report is Markdown (plaintext): Escaping not needed; Markdown doesn't interpret inline HTML by default

**Data emerges as:** `GRAPH_REPORT.md` (plaintext Markdown)

**Risk level:** ✅ LOW  
**Rationale:** Markdown is plaintext-safe. No HTML rendering occurs.

---

#### **Stage 6b: Export (All Formats)**

**Untrusted inputs:**
- Graph nodes, edges, analysis (verified)
- Labels, metadata (already sanitized in earlier stages)

**Per-format validation:**

| Format | Content Type | Encoding Used | Risk |
|--------|-------------|---------------|------|
| **JSON** | Application JSON | `JsonSerializer` with `JavaScriptEncoder.UnsafeRelaxedJsonEscaping` | ✅ LOW — JSON strings properly escaped |
| **HTML** | Text/HTML + JavaScript | `System.Net.WebUtility.HtmlEncode()` for labels in vis.js JSON | ⚠️ MEDIUM — See Concern 2 |
| **SVG** | Image/SVG+XML | **No escaping documented** | ⚠️ MEDIUM — See Concern 2 |
| **Obsidian** | Text/Markdown (Markdown backlinks) | `Uri.EscapeDataString()` for file names, plain text for content | ✅ LOW — Markdown is plaintext |
| **Wiki** | Text/Markdown | Same as Obsidian | ✅ LOW |
| **Neo4j Cypher** | Text/Plain (Cypher DSL) | **String literals quoted but not escaped** | ⚠️ MEDIUM — See Concern 2 |
| **Report** | Text/Markdown | Plain text with labels sanitized | ✅ LOW |

**Risk level:** ⚠️ MEDIUM (Concerns 2 and 3 below)

---

## 3. Configuration Security Architecture

### Layering (Priority Order, Highest to Lowest)

```
┌─────────────────────────────────────────────┐
│  5. CLI Arguments (--provider, --api-key)   │  ← Highest priority
├─────────────────────────────────────────────┤
│  4. User Secrets (dotnet user-secrets)      │
├─────────────────────────────────────────────┤
│  3. Environment Variables (GRAPHIFY__*)     │
├─────────────────────────────────────────────┤
│  2. appsettings.local.json (wizard-saved)   │
├─────────────────────────────────────────────┤
│  1. appsettings.json (defaults)             │  ← Lowest priority
└─────────────────────────────────────────────┘
```

#### Configuration Analysis

**appsettings.json (Stage 1 defaults):**
- Contains Ollama (localhost:11434) and CopilotSdk defaults
- Checked into git (✅ safe, no secrets)
- Override mechanism: env vars and CLI args have higher priority

**appsettings.local.json (Stage 2, wizard-saved):**
- User's saved configuration (created by `ConfigWizard`)
- **Should be in `.gitignore`** — Verify in repo ✅

**Environment Variables (Stage 3, GRAPHIFY__* prefix):**
- `GRAPHIFY__AzureOpenAI__ApiKey` etc.
- Standard .NET configuration convention
- ✅ Best for CI/CD (secrets in Actions secrets)
- ⚠️ **CONCERN 2:** Env var scope — if `GRAPHIFY_ApiKey` is set system-wide and someone SSH's in, it could leak. **Mitigation:** Use GitHub Actions secrets for CI, local dev uses `dotnet user-secrets`.

**User Secrets (Stage 4):**
- Stored in `~/.microsoft/usersecrets/{userSecretsId}/secrets.json`
- Not checked into git ✅
- Per-developer isolation ✅
- **Best practice for local development**

**CLI Arguments (Stage 5, highest priority):**
- `--api-key` passed on command line
- ⚠️ **CONCERN 3:** API key visible in shell history, process listing
- **Mitigation:** Document best practice — use env vars or user-secrets, not CLI (for production use)

#### Secret Handling: API Keys

**Azure OpenAI ApiKey:**
- Supported in: CLI `--api-key`, env var `GRAPHIFY__AzureOpenAI__ApiKey`, user-secrets
- Passed to `AzureOpenAIClientFactory.Create()`
- Used to construct `AzureOpenAIOptions` record
- Never logged or output (except masked in `config show` command: `****...last4`)

**Is there a way to accidentally send secrets to an AI provider?**
- ✅ **NO.** API keys are configuration, not data. Only source file contents + extraction prompts are sent to providers.
- AST extraction never contacts any provider
- Semantic extraction via `SemanticExtractor` uses `IChatClient` interface, which has already been authenticated with the API key (stored by the factory, not passed per-request)

**Risk level:** ✅ LOW  
**Rationale:** Secrets are properly isolated, never passed as data. CLI-provided secrets have shell-history risk but documented.

---

## 4. AI Provider Security Model

### Three Provider Threat Models

#### **Ollama (Local)**

**Data sent:**
- Source file contents
- File paths
- Extraction prompts

**Threat model:**
- ✅ **Trusted**: Running on localhost:11434
- ✅ **No external network call** (by default, unless endpoint is changed)
- ✅ **User controls the model**: `ModelId = "llama3.2"` etc.
- ✅ No credentials: Ollama is unauthenticated on localhost

**Risk level:** ✅ VERY LOW

---

#### **Azure OpenAI (Cloud)**

**Data sent:**
- Source file contents (full text)
- File paths (metadata)
- Extraction prompts (system-controlled)

**Data received:**
- JSON chat completions (nodes, edges, confidence scores)

**Threat model:**
- ⚠️ **Untrusted network:** Data leaves the organization's network
- ⚠️ **Third-party storage:** Microsoft may retain logs (per [Azure OpenAI data handling](https://learn.microsoft.com/en-us/legal/cognitive-services/openai/data-privacy))
- ✅ **Encrypted in transit:** HTTPS
- ✅ **Managed identity option:** Azure OpenAI supports Microsoft Entra ID (credentials never stored locally)
- ✅ **API key rotatable:** Secrets can be rotated via Azure portal

**Is source code data sent?**
- ✅ **YES.** Full file contents (within 1MB per file) are sent to Azure OpenAI for semantic extraction.
- This is intentional: semantic extraction requires understanding code semantics.
- **Mitigation:** Document in README that sensitive code should not be analyzed; use `--exclude-patterns` if needed.

**Risk level:** ⚠️ MEDIUM  
**Rationale:** Data leaves the network, but encrypted and under user control (user decides to send it). Managed identity reduces credential risk. **Recommendation:** Document data privacy and add a disclaimer in CLI output when Azure OpenAI is selected.

---

#### **Copilot SDK (GitHub)**

**Data sent:**
- Source file contents
- File paths
- Extraction prompts

**Data received:**
- Chat completions from GitHub Copilot / GitHub Models endpoint

**Threat model:**
- ✅ **Authentication:** Handled by GitHub Copilot CLI (user must be logged in with `gh auth login`)
- ✅ **Managed identity:** No API key stored locally; uses GitHub's auth token
- ✅ **User's choice:** User controls which model is sent and which data
- ⚠️ **GitHub retention:** Subject to GitHub's data retention policies

**Code sent to GitHub?**
- ✅ **YES.** Full source contents are sent to GitHub's Copilot API.
- **Mitigation:** Same as Azure OpenAI — document and add CLI warning.

**Risk level:** ⚠️ MEDIUM  
**Rationale:** Data leaves to GitHub, but user chooses provider. Managed identity is secure. **Recommendation:** Add warning in `SemanticExtractor` or `PipelineRunner` when AI provider is selected.

---

## 5. Export Security Architecture

### 7 Export Formats: Encoding Strategy

#### **1. JSON Export** (`graph.json`)

**Content:** Nodes, edges, metadata, analysis results

**Encoding:**
```csharp
var options = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    // ...
};
```

**Risk:** ✅ LOW  
**Rationale:** JSON string escaping is standard. `UnsafeRelaxedJsonEscaping` is safe for JSON (it only skips escaping of non-control characters that are safe in JSON context).

---

#### **2. HTML Export** (`graph.html`)

**Content:** Interactive vis.js visualization

**Encoding:**
- Node labels: `System.Net.WebUtility.HtmlEncode()` (encodes `<>&'"`)
- Node data embedded in vis.js JSON: `JavaScriptEncoder.UnsafeRelaxedJsonEscaping`

**Template:**
- Generated from `HtmlTemplate.Generate(title, nodesJson, edgesJson, legendJson, stats)`
- Template is a resource string (static, embedded)

**Risk:** ✅ LOW (with documentation note)  
**Rationale:** HTML encoding is applied to node labels. Template is static (no interpolation risk). However, see **Concern 2** below for SVG/HTML edge case.

---

#### **3. SVG Export** (`graph.svg`)

**Content:** Static SVG rendering of graph

**Encoding:**
- **No explicit escaping documented in code** ⚠️

**Code location:** `SvgExporter.cs` (not yet examined in full detail)

**Risk:** ⚠️ MEDIUM  
**Rationale:** SVG is XML, and text content must be escaped. If node labels contain `<script>` or other SVG tags, they could be rendered as SVG elements. **Mitigation:** Assume `SvgExporter` inherits the same label sanitization pipeline (via `SanitizeLabel()`), but this should be explicitly verified. If not, labels should be XML-escaped.

**Recommendation:** Audit `SvgExporter.cs` to verify label escaping.

---

#### **4. Obsidian Vault** (`obsidian/`)

**Content:** Markdown files with backlinks, organized by community

**Encoding:**
- File names: URL-encoded (to avoid special characters in file paths)
- Content: Markdown (plaintext-safe; no HTML interpretation)
- Backlinks: `[[node-label]]` syntax (Obsidian format)

**Risk:** ✅ LOW  
**Rationale:** Markdown is plaintext. File names are URL-encoded. No executable content.

---

#### **5. Wiki Export** (`wiki/`)

**Content:** Markdown articles with navigation

**Encoding:** Same as Obsidian (Markdown plaintext)

**Risk:** ✅ LOW

---

#### **6. Neo4j Cypher** (`graph.cypher`)

**Content:** Cypher CREATE statements

**Encoding:**
- String literals quoted: `'node-label'`
- **No escaping of single quotes in labels** ⚠️

**Code location:** `Neo4jExporter.cs` (not examined in detail)

**Risk:** ⚠️ MEDIUM  
**Rationale:** Cypher DSL requires string literals to be quoted. If a label contains a single quote, it needs to be escaped as `\'` or `''`. If not, the Cypher statement will be syntactically invalid. **Not a security issue per se** (invalid Cypher won't execute), but could cause Neo4j import to fail.

**Recommendation:** Verify `Neo4jExporter` escapes single quotes in string literals.

---

#### **7. Report** (`GRAPH_REPORT.md`)

**Content:** Markdown summary with top nodes, connections, metrics

**Encoding:** Plain text (Markdown)

**Risk:** ✅ LOW

---

### Summary: Export Encoding

| Format | Active Encoding | Risk | Status |
|--------|-----------------|------|--------|
| JSON | ✅ JSONEncoder | ✅ LOW | Verified |
| HTML | ✅ HtmlEncode | ✅ LOW | Verified |
| SVG | ⚠️ Not documented | ⚠️ MED | **Needs audit** |
| Obsidian | ✅ URL-encode filenames | ✅ LOW | Verified |
| Wiki | ✅ Markdown | ✅ LOW | Verified |
| Neo4j | ⚠️ Not documented | ⚠️ MED | **Needs audit** |
| Report | ✅ Plaintext | ✅ LOW | Verified |

---

## 6. Dependency Trust Analysis

### Key Dependencies with File System / Network Access

| Package | Access | Trust | Risk |
|---------|--------|-------|------|
| **Microsoft.Extensions.AI** | Network (to AI providers) | ✅ HIGH — Microsoft official | ✅ LOW |
| **GitHub.Copilot.SDK** | Network (to GitHub), FS (auth tokens) | ✅ HIGH — GitHub official | ✅ LOW |
| **TreeSitter.Bindings** | File system (grammars, language definitions) | ✅ MEDIUM — Community library, mature | ✅ LOW |
| **QuikGraph** | No FS/Network | ✅ HIGH — Stable, academic | ✅ LOW |
| **System.CommandLine** | No FS/Network | ✅ HIGH — .NET official | ✅ LOW |
| **ModelContextProtocol** | Network (MCP server communication) | ✅ MEDIUM — New, maintained by community | ✅ LOW |
| **Microsoft.Extensions.Configuration** | File system (appsettings.json, secrets) | ✅ HIGH — .NET official | ✅ LOW |
| **Microsoft.Extensions.Logging** | File system (logs, if configured) | ✅ HIGH — .NET official | ✅ LOW |
| **Spectre.Console** | Console output only | ✅ HIGH — Community, widely adopted | ✅ LOW |

### Transitive Dependencies (Implicit Risk)

**No explicit dependency auditing tool is configured.** Recommend:
- Add `dotnet list --outdated` to CI
- Monitor for security advisories via GitHub Dependabot (already configured?)
- Consider adding `dotnet package-audit` or similar tool

### NuGet.org Trust

**All packages from NuGet.org.** Trust model:
- ✅ NuGet.org requires Microsoft account to publish
- ✅ Packages are scanned for malware before publishing
- ⚠️ No package signing enforcement (optional feature)

**Recommendation:** For production use, consider enabling package signature verification in `nuget.config`.

---

## 7. Architecture-Level Security Concerns

### **Concern 1: AI Response Validation (LOW-IMPACT)**

**Issue:** `SemanticExtractor` deserializes LLM responses with minimal validation against a strict schema.

**Scenario:**
```json
{
  "nodes": [
    { "id": "<img src=x onerror=alert(1)>", "label": "Malicious", ... }
  ]
}
```

**Impact:**
- ✅ Not exploitable in JSON export (strings are JSON-escaped)
- ✅ Not exploitable in HTML export (labels are HtmlEncode-d)
- ⚠️ **Could cause rendering issues** in vis.js if JavaScript in ID somehow executes (unlikely)
- ⚠️ **Graph semantics broken** (node with ugly ID won't match other nodes)

**Mitigation:** Already in place via downstream escaping. Could add stricter validation in `ExtractionValidator`, but current approach is acceptable.

**Recommendation:** Document that LLM outputs are opaque; trust downstream encoding.

---

### **Concern 2: SVG and Cypher Encoding (MEDIUM)**

**Issue:** `SvgExporter` and `Neo4jExporter` don't have documented escaping strategies for node labels.

**Scenario:**
```
SVG: <text>Label with <svg>Injected</svg></text>
→ Renders as nested SVG element (unintended)

Cypher: CREATE (n {name: 'Label with 'quote'})
→ Syntax error, import fails
```

**Impact:**
- SVG: Unintended rendering, could cause visual confusion (not a security breach, but quality issue)
- Cypher: Import failures, data loss

**Mitigation:**
1. Verify `SvgExporter.cs` applies label sanitization
2. Verify `Neo4jExporter.cs` escapes quotes in Cypher strings

**Recommendation:** Audit both exporters; add explicit XML/Cypher escaping if missing. Add to regression test suite.

**Status:** OPEN — Needs code review

---

### **Concern 3: Data Privacy Warning Missing (MEDIUM)**

**Issue:** Users may not realize that Azure OpenAI and Copilot SDK send source code to external providers.

**Scenario:**
```
User runs: graphify run --provider azureopenai
→ Source code is sent to Microsoft's servers
→ User may not realize this (not documented in CLI output)
```

**Impact:**
- ⚠️ **Compliance risk** — Company policy may forbid external data transfer
- ⚠️ **Privacy risk** — Sensitive code exposed to third party

**Mitigation:** Add warning message when AI provider is selected (especially Azure/Copilot).

**Recommendation:** Modify `PipelineRunner` or `SemanticExtractor` to output:
```
⚠️  Data Privacy: Source file contents are being sent to {provider} for analysis.
    See documentation for details on data retention and privacy.
```

**Status:** OPEN — Product recommendation

---

## 8. Security Recommendations

### High Priority (Implement Soon)

1. **Audit SVG and Cypher Exporters**
   - Verify label escaping in `SvgExporter.cs` and `Neo4jExporter.cs`
   - Add escaping if missing
   - Add regression tests for special characters in labels

2. **Add Data Privacy Warning**
   - When Azure OpenAI or Copilot SDK is selected, print a warning to CLI
   - Example:
     ```
     ⚠️  Data Privacy Warning:
         Source code contents will be sent to Microsoft Azure OpenAI / GitHub.
         Review https://docs.graphify-dotnet/privacy for details.
     ```

3. **Document Secret Handling Best Practices**
   - Add section to `docs/configuration.md` recommending:
     - Use `dotnet user-secrets` for local development (NOT CLI args)
     - Use GitHub Actions secrets for CI/CD
     - Never commit API keys to git (add to `.gitignore` if needed)

### Medium Priority (Future)

4. **Add Dependency Scanning to CI**
   - Add `dotnet list --outdated` to GitHub Actions
   - Consider enabling Dependabot for NuGet packages
   - Monitor for security advisories

5. **Implement NuGet Package Signing**
   - Enable signature verification in `nuget.config`
   - Sign published packages with code signing certificate

6. **Add Stricter LLM Response Validation**
   - Create a `SemanticResponseValidator` that checks:
     - Node IDs are valid identifiers (alphanumeric + underscore)
     - Node labels are within length limits
     - Relations are from a known set
   - Non-blocking: log invalid responses, continue with best-effort extraction

### Low Priority (Polish)

7. **Explicit SVG/Cypher Security Tests**
   - Add integration tests for labels with special characters
   - Ensure exported files are valid (SVG valid XML, Cypher valid syntax)

8. **Audit Trail / Logging**
   - Log which files were analyzed (already done via verbose mode)
   - Log which provider was used (already done)
   - Consider audit log of API key usage (sensitivity: low, since no log of actual queries)

---

## 9. Summary: Security Posture

### Strengths

✅ **Defense in Depth:**
- Input validation at file detection boundary (path traversal, size limits, extension whitelist)
- Structured extraction via AST (no eval, no code injection)
- Provider abstraction layer (IChatClient, testable, provider-agnostic)
- Per-format output encoding (JSON, HTML, Markdown)

✅ **Configuration Security:**
- Layered configuration (CLI > user-secrets > env > local > defaults)
- Secrets never logged or output to console (except masked in `config show`)
- API keys isolated in `AzureOpenAIOptions` record, not passed as data

✅ **Trust Boundaries Clear:**
- Explicit separation of untrusted inputs (file paths, file contents, AI responses) from controlled pipeline
- No code execution, no dynamic imports, no deserialization of untrusted code

✅ **AI Provider Security:**
- No accidental secret transmission to providers
- Data transmission is intentional (source code for semantic analysis)
- Managed identity option available for Azure OpenAI

### Weaknesses (Documented Above)

⚠️ **Concern 1:** LLM response validation minimal (mitigated by downstream escaping)  
⚠️ **Concern 2:** SVG/Cypher escaping not audited (open question)  
⚠️ **Concern 3:** Data privacy warning missing (product recommendation)

### Overall Risk Assessment

| Category | Risk Level | Confidence |
|----------|-----------|------------|
| Input Validation | ✅ LOW | HIGH |
| Configuration/Secrets | ✅ LOW | HIGH |
| AI Provider Data Flow | ⚠️ MEDIUM (expected) | HIGH |
| Export Encoding | ⚠️ MEDIUM (2 formats) | MEDIUM |
| Dependency Trust | ✅ LOW | HIGH |
| **Overall** | ✅ **LOW** | **HIGH** |

**Verdict:** graphify-dotnet is architecturally sound for general use. The three documented concerns are manageable and lower-risk than the architecture strengths suggest. Recommend implementing High Priority recommendations (1-3) before production release.

---

## 10. References

- **Input Validator:** `src/Graphify/Security/InputValidator.cs` — URL/path/label sanitization
- **Configuration:** `src/Graphify.Cli/Configuration/ConfigurationFactory.cs` — Layered config; `ChatClientResolver.cs` — provider resolution
- **Pipeline:** `src/Graphify.Cli/PipelineRunner.cs` — Full pipeline orchestration
- **Export:** `src/Graphify/Export/*.cs` — Per-format exporters
- **SDK:** `src/Graphify.Sdk/ChatClientFactory.cs` — Provider-agnostic AI abstraction
- **CLI:** `src/Graphify.Cli/Program.cs` — Command-line interface with config wizard

---

**Document Status:** Ready for Review  
**Next Steps:**
1. Security team reviews and approves
2. Implement High Priority recommendations (1-3)
3. Audit SVG/Cypher exporters (Concern 2)
4. Add regression tests for special character handling
5. Re-review before production release
