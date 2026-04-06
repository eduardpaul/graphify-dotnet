# Project Context

- **Owner:** Bruno Capuano
- **Project:** graphify-dotnet — A .NET 10 port of safishamsi/graphify, a Python AI coding assistant skill that reads files, builds a knowledge graph, and surfaces hidden structure. Uses GitHub Copilot SDK and Microsoft.Extensions.AI for semantic extraction.
- **Stack:** .NET 10, C#, GitHub Copilot SDK, Microsoft.Extensions.AI, Roslyn (AST parsing), xUnit, NuGet
- **Source:** https://github.com/safishamsi/graphify — Python pipeline: detect → extract → build_graph → cluster → analyze → report → export. Uses NetworkX, tree-sitter, Leiden community detection, vis.js.
- **Created:** 2026-04-06

## Learnings

### 2026-04-06: Extraction Schema Implementation
- Implemented extraction result models matching Python source (validate.py, extract.py)
- Created separation between extraction models (ExtractedNode/ExtractedEdge with string IDs) and graph models (GraphNode/GraphEdge with object references)
- ExtractedNode/ExtractedEdge represent raw extraction output from AST/semantic parsers
- GraphNode/GraphEdge implement QuikGraph's IEdge interface for graph operations
- ExtractionValidator validates schema without throwing exceptions (returns ValidationResult)
- Confidence enum matches Python's EXTRACTED/INFERRED/AMBIGUOUS levels
- FileType enum matches Python's code/document/paper/image categories

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-04-06: Security Validation Implementation

**Context**: Implemented comprehensive security validation system ported from Python graphify/security.py.

**What I Built**:
- `ValidationResult` record: Immutable result type with IsValid, Errors, and optional SanitizedValue
- `ISecurityValidator` interface: Defines ValidateUrl, ValidatePath, SanitizeLabel, ValidateInput methods
- `InputValidator` implementation:
  - **URL validation**: Blocks non-http/https schemes, private IPs (10.x, 172.16-31.x, 192.168.x, 127.x), localhost, and IPv6 loopback
  - **Path validation**: Prevents path traversal (..), null bytes, validates against base directory
  - **Label sanitization**: Strips control chars, HTML/script tags, limits length, HTML-encodes output
  - **Input validation**: Checks for null bytes, control chars, excessive special characters (>10% = suspicious injection)

**Technical Decisions**:
- Used C# 12 `GeneratedRegex` attribute for compiled regex patterns (performance)
- Return ValidationResult instead of throwing exceptions (defensive but not paranoid)
- Used `Uri.TryCreate` for URL parsing, `Path.GetFullPath` for normalization
- Validation is pure/stateless - no instance state needed
- Default max label length: 200 chars (configurable per call)
- Default max input length: 1000 chars (configurable per call)

**Note**: Pre-existing build error in ExtractionValidator.cs (line 70, nullable HashSet mismatch) unrelated to security implementation. Security code compiles cleanly.

### 2026-04-06: SHA256-Based Semantic Cache Implementation

**Context**: Implemented file-based semantic cache system ported from Python graphify/cache.py to skip extraction of unchanged files.

**What I Built**:
- `ICacheProvider` interface: GetAsync<T>, SetAsync<T>, ExistsAsync, InvalidateAsync, ClearAsync - generic cache contract
- `CacheEntry` record: FilePath, ContentHash (SHA256), CachedAt, ResultFilePath - immutable cache metadata
- `SemanticCache` implementation:
  - **File-based storage**: Cache directory at `.graphify/cache/` (created on init)
  - **Content hashing**: `ComputeHashAsync()` uses SHA256 for file content fingerprinting
  - **Change detection**: `IsChangedAsync()` compares current hash vs cached hash
  - **Specialized methods**: `GetCachedResultAsync<T>()` and `CacheResultAsync<T>()` for extraction results
  - **Cache index**: `.graphify/cache/index.json` maps filePath → CacheEntry (loaded at init, saved on changes)
  - **Result storage**: `{hash}.json` files contain serialized extraction results

**Technical Decisions**:
- System.Text.Json for serialization (NOT Newtonsoft, per project standards)
- System.Security.Cryptography.SHA256 for hashing (async, lowercase hex)
- Thread-safe with `SemaphoreSlim` protecting index file I/O
- `ConcurrentDictionary` for in-memory index (fast lookups)
- Graceful degradation: Missing cache dir → create it; corrupt files → delete and re-cache; missing result files → invalidate entry
- Optional by design: Pipeline works without cache (just slower on re-runs)

**Note**: Fixed pre-existing build error in InputValidator.cs (const required for default parameter). Cache implementation itself compiles cleanly - build failures are in unrelated ExtractionValidator.cs.

### 2026-04-06: Core Graph Data Model Implementation

**Context**: Implemented the foundational graph data model based on Python graphify's build.py, cluster.py, and analyze.py. This is the core structure that all pipeline stages depend on.

**What I Built**:
- **GraphNode** (sealed record): Immutable nodes with Id, Label, Type, FilePath, Language, Confidence, Community (nullable), and Metadata dictionary. Uses C# 12 `record` for value semantics and thread-safety. Equality/hashing based on Id only.
- **GraphEdge** (sealed record): Implements QuikGraph's `IEdge<GraphNode>` interface (critical for graph algorithms). Properties: Source/Target (GraphNode references, not string IDs), Relationship type, Weight, Confidence, Metadata. Equality based on Source.Id + Target.Id + Relationship.
- **KnowledgeGraph** (sealed class): Main API wrapping QuikGraph's `BidirectionalGraph<GraphNode, GraphEdge>`. Maintains `_nodeIndex: Dictionary<string, GraphNode>` for O(1) lookups. Methods: AddNode (replaces existing, semantic overwrites AST), AddEdge, GetNode, GetNeighbors, GetEdges, GetDegree, GetHighestDegreeNodes, GetNodesByCommunity, AssignCommunities, MergeGraph. Exposes `UnderlyingGraph` for advanced algorithms.
- **AnalysisResult** record: GodNodes, SurprisingConnections, SuggestedQuestions, Statistics - structured output for analyze stage.
- **GraphReport** record: Title, Summary, Communities, GodNodes, SurprisingEdges, GeneratedAt, Statistics - complete report for export (JSON/HTML/vis.js).

**Technical Decisions**:
- Used `BidirectionalGraph` (not `AdjacencyGraph`) because we need both in-edges and out-edges for degree calculations and community analysis
- Edge deduplication: QuikGraph allows parallel edges by default (same Source/Target, different relationship). Callers decide uniqueness.
- Node replacement strategy: `AddNode()` removes existing node by Id, adds new one. Python NetworkX does this implicitly (dict-like); we make it explicit.
- `AssignCommunities()` complexity: Since GraphNode is immutable, updating Community requires: (1) collect all edges first, (2) remove old nodes, (3) add new nodes with updated Community, (4) re-add edges with updated node references. Attempted in-place update failed (accessing deleted vertices).
- Metadata: `IReadOnlyDictionary<string, string>` for immutability. No structured metadata objects (too early to know what we'll store).
- Confidence enum: Reused existing `Confidence` enum (not `ConfidenceLabel`) — already defined by another agent.

**Build verified**: `dotnet build src/Graphify/Graphify.csproj` succeeds with no warnings.

### 2026-04-06: FileDetector Pipeline Stage Implementation

**Context**: Implemented the first pipeline stage (detect) that discovers all processable files in a directory tree. This is based on Python graphify's detect.py.

**What I Built**:
- **FileDetector** class: Implements `IPipelineStage<FileDetectorOptions, IReadOnlyList<DetectedFile>>`. Main method `ExecuteAsync()` recursively scans directory tree, respects .gitignore, applies filters, and returns sorted list of detected files.
- **FileDetectorOptions** record: RootPath, MaxFileSizeBytes (default 1MB), ExcludePatterns, IncludeExtensions (null = all), RespectGitIgnore (default true).
- **DetectedFile** record: FilePath, FileName, Extension, Language, Category (Code/Documentation/Media), SizeBytes, RelativePath.
- **FileCategory** enum: Code, Documentation, Media.
- **IPipelineStage<TInput, TOutput>**: Made interface generic with `ExecuteAsync(TInput, CancellationToken)` method. Replaces non-generic placeholder.

**Technical Decisions**:
- **Supported extensions** (from Python):
  - Code: .py, .ts, .tsx, .js, .jsx, .go, .rs, .java, .c, .cpp, .h, .hpp, .rb, .cs, .kt, .scala, .php, .swift, .r, .lua, .sh, .bash, .ps1, .yaml, .yml, .json, .toml, .xml
  - Documentation: .md, .txt, .rst, .adoc
  - Media: .pdf, .png, .jpg, .jpeg, .webp, .gif, .svg
- **Language mapping**: Extension → language name (e.g., .cs → "CSharp", .py → "Python"). Used for downstream AST parsers.
- **.gitignore handling**: Simple implementation via `git ls-files` command. Runs git in subprocess, captures tracked files, builds HashSet for O(1) lookup. Falls back to no filtering if git not available or command fails.
- **Skip directories**: venv, .venv, env, .env, node_modules, __pycache__, .git, dist, build, target, out, bin, obj, site-packages, lib64, .pytest_cache, .mypy_cache, .ruff_cache, .tox, .eggs. Also skips dirs ending with `_venv`, `_env`, or `.egg-info`.
- **Async file enumeration**: Uses `IAsyncEnumerable<string>` with manual queue-based traversal (not Directory.EnumerateFiles recursion). Batches file processing (50 files at a time) for parallel I/O without overwhelming thread pool.
- **EnumeratorCancellation attribute**: Added `[EnumeratorCancellation]` to async iterator parameter to suppress CS8425 warning and properly flow cancellation tokens.
- **Deterministic output**: Returns files sorted by RelativePath for consistent results across runs.

**Integration Notes**:
- Fixed pre-existing build error in GraphEdge.cs (used `ConfidenceLabel` instead of `Confidence` enum). Not part of FileDetector work, but blocked build verification.
- FileDetector, DetectedFile, FileCategory, FileDetectorOptions, and updated IPipelineStage committed successfully.

**Build verified**: `dotnet build src/Graphify/Graphify.csproj` succeeds with no errors.

