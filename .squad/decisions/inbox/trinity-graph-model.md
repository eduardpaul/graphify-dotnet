# Decision: Core Graph Data Model Architecture

**Author:** Trinity (Core Developer)  
**Date:** 2026-04-06  
**Status:** Proposed

## Context

The graphify-dotnet pipeline requires a graph data structure to represent code relationships. The Python version uses NetworkX (pure Python, dict-based nodes). We need a .NET equivalent that:
1. Supports graph algorithms (degree, betweenness, clustering)
2. Works with immutable records for thread-safety
3. Handles node/edge metadata flexibly
4. Performs well for graphs with 100-10,000 nodes

## Decision

Use **QuikGraph's BidirectionalGraph<GraphNode, GraphEdge>** wrapped in a custom `KnowledgeGraph` class.

### Why QuikGraph?
- **Mature library**: 2.x stable, maintained, targets modern .NET
- **Algorithm library**: Betweenness centrality, shortest paths, topological sort — we'll need these for analysis
- **Generic design**: Works with any `IEdge<TVertex>` implementation
- **Bidirectional**: O(1) access to both in-edges and out-edges (critical for degree calculations)

### Why wrap it?
- **Domain API**: Methods like `GetNodesByCommunity()`, `AssignCommunities()`, `MergeGraph()` hide graph theory from pipeline code
- **Node indexing**: Maintain `Dictionary<string, GraphNode>` for O(1) lookup by string Id (QuikGraph only indexes by vertex reference)
- **Node replacement**: NetworkX allows `G.add_node(id, **attrs)` to overwrite. QuikGraph requires explicit remove+add. Wrapper provides this semantic.
- **Future-proofing**: If we swap graph libraries later, only `KnowledgeGraph.cs` changes

### Alternatives Considered

1. **Pure Dictionary<string, GraphNode> + List<GraphEdge>**: Simple, but we'd reimplement graph algorithms. Not worth it.
2. **NetworkX.NET** (if it existed): Doesn't exist. Python NetworkX is pure Python, no .NET port.
3. **AdjacencyGraph** (QuikGraph): No in-edge access (only out-edges). Can't efficiently compute degree or find reverse relationships.

## Implications

- **Immutability cost**: Updating node properties (e.g., assigning communities) requires remove+add cycle for all affected nodes and edges. Acceptable because clustering happens once per pipeline run.
- **Edge storage**: Parallel edges allowed (same Source/Target, different Relationship). Deduplication is caller's responsibility if needed.
- **Metadata schema**: No typed metadata objects yet. Use `IReadOnlyDictionary<string, string>` until we know what's needed.

## Open Questions

1. **Hyperedges**: Python graphify has `hyperedges` list (N-to-M relationships). QuikGraph doesn't support this natively. Store as separate list in metadata?
2. **Graph serialization**: Do we serialize the entire QuikGraph or just nodes+edges as JSON? Likely the latter (export stage concern).
3. **Community assignment mutability**: Should we store community assignments in a separate `Dictionary<string, int>` instead of mutating nodes? Current approach is simple but expensive.

## Review Needed

- **Neo (Architect)**: Approve graph library choice and wrapper strategy?
- **Morpheus (Pipeline)**: Does `KnowledgeGraph` API cover pipeline needs (build → cluster → analyze)?
