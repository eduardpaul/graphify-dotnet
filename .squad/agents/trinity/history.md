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

