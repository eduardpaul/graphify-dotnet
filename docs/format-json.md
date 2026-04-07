# JSON Graph Export

> Machine-readable graph data in a standard format for custom tools and CI pipelines.

## Quick Start

```bash
graphify run ./my-project --format json
# Generates graph.json with complete graph structure
```

## What it Produces

The **JSON format** generates `graph.json` containing:
- **Nodes** — All extracted entities (classes, functions, modules, concepts)
- **Edges** — All relationships (calls, references, imports, semantic connections)
- **Metadata** — Communities, confidence scores, extraction sources
- **Statistics** — Node and edge counts, community breakdown

## JSON Schema

```json
{
  "nodes": [
    {
      "id": "userrepository",
      "label": "UserRepository.cs",
      "type": "Entity",
      "community": 4,
      "file_path": "samples/mini-library/src/UserRepository.cs",
      "confidence": "EXTRACTED",
      "metadata": {
        "source_location": "L1"
      }
    }
  ],
  "edges": [
    {
      "source": "userservice",
      "target": "userrepository",
      "relationship": "calls",
      "weight": 1,
      "confidence": "EXTRACTED",
      "metadata": {
        "merge_count": "1",
        "source_file": "samples/mini-library/src/UserService.cs"
      }
    }
  ],
  "metadata": {
    "node_count": 47,
    "edge_count": 79,
    "community_count": 7,
    "generated_at": "2026-04-07T02:14:03Z"
  }
}
```

### Field Reference

**Nodes:**
- `id` — Unique identifier (e.g., "userrepository", "userservice_getbyid")
- `label` — Display name (e.g., "UserRepository.cs", "GetById()")
- `type` — Node type (e.g., Entity, Class, Function, Module, File, Concept)
- `community` — Integer community ID (null if unassigned)
- `file_path` — Source file path
- `language` — Programming language (if detected)
- `confidence` — Extraction confidence ("EXTRACTED", "INFERRED", "AMBIGUOUS")
- `metadata` — Optional key-value pairs (source_location, merge_count, etc.)

**Edges:**
- `source` — Source node ID
- `target` — Target node ID
- `relationship` — Type of relationship (calls, references, imports, extends, implements, contains, semantic, inferred)
- `weight` — Numeric edge weight (e.g., number of references between same nodes)
- `confidence` — Extraction confidence ("EXTRACTED", "INFERRED", "AMBIGUOUS")
- `metadata` — Optional key-value pairs (merge_count, source_file, etc.)

**Metadata:**
- `node_count` — Total number of nodes in the graph
- `edge_count` — Total number of edges in the graph
- `community_count` — Number of detected communities
- `generated_at` — ISO 8601 timestamp of when the graph was generated

## How to Use

### Load into Custom Tools

```csharp
// C# example
var json = System.IO.File.ReadAllText("graph.json");
var graph = System.Text.Json.JsonSerializer.Deserialize<GraphData>(json);
foreach (var node in graph.Nodes)
{
    Console.WriteLine($"{node.Id}: {node.Label}");
}
```

```javascript
// JavaScript example
const graph = await fetch('graph.json').then(r => r.json());
console.log(`Graph has ${graph.metadata.node_count} nodes`);
```

```python
# Python example
import json
with open('graph.json') as f:
    graph = json.load(f)
for node in graph['nodes']:
    print(f"{node['id']}: {node['label']}")
```

### Pipe to Other Tools

```bash
# Extract just node names
jq '.nodes[].label' graph.json

# Find all nodes in a specific community
jq '.nodes[] | select(.community == 0)' graph.json

# List all external imports
jq '.edges[] | select(.relationship == "imports")' graph.json
```

### Use in CI/CD Pipelines

```yaml
# GitHub Actions example
- name: Analyze graph
  run: |
    graphify run ./src --format json
    jq '.nodes | length' graph.json  # node count
    jq '.metadata.community_count' graph.json  # community count
```

### Visualize Elsewhere

Build custom visualizations using D3.js, Three.js, Cytoscape.js, or any tool that accepts JSON:

```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/cytoscape.js/3.24.0/cytoscape.min.js"></script>
<script>
  const graph = await fetch('graph.json').then(r => r.json());
  const cy = cytoscape({
    container: document.getElementById('cy'),
    elements: [
      ...graph.nodes.map(n => ({ data: { id: n.id, label: n.label } })),
      ...graph.edges.map(e => ({ data: { source: e.source, target: e.target } }))
    ]
  });
</script>
```

## Best For

- **Custom tooling** — Build your own analysis and visualization
- **CI/CD integration** — Automated graph generation and metrics
- **Data science** — Load into Pandas, NetworkX, or R for analysis
- **API integration** — Feed graph data to other systems
- **Reproducibility** — Store as machine-readable source of truth

## Example Use Cases

### Dependency Analysis
Query the JSON to find circular dependencies, unused modules, or tightly coupled components.

### Metrics Tracking
Extract node counts, community sizes, and average degree to track codebase growth.

### Architecture Validation
Use jq or custom scripts to assert that certain nodes don't connect across community boundaries.

### AI Agent Context
Feed graph.json to Claude, ChatGPT, or Copilot as context for code review or refactoring suggestions.

## Performance Notes

- Files up to 10,000 nodes typically <5 MB
- Load time is linear with node count
- Compression: gzip reduces size by ~70% (minimal impact on parsing)

## See Also

- [Worked Example](worked-example.md) — Real output from a C# project walkthrough
- [Export Formats Overview](export-formats.md)
- [HTML Interactive Viewer](format-html.md) — Visual exploration of the same data
- [Neo4j Cypher Export](format-neo4j.md) — For advanced graph queries and databases
