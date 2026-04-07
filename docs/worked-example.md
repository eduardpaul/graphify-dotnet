# Worked Example

The `samples/mini-library/` directory contains a complete worked example — a small C# library demonstrating the repository pattern. Run the pipeline and see all 7 output formats.

## Run It

```bash
# Using the global tool
graphify run samples/mini-library --format json,html,svg,neo4j,obsidian,wiki,report

# Or build from source
dotnet run --project src/Graphify.Cli -- run samples/mini-library --format json,html,svg,neo4j,obsidian,wiki,report -v
```

## Output

Pre-generated output is available at [`samples/mini-library/graphify-out/`](../samples/mini-library/graphify-out/):

```
samples/mini-library/graphify-out/
├── GRAPH_REPORT.md    # Analysis report (god nodes, communities, insights)
├── graph.json         # Full graph data (47 nodes, 79 edges)
├── graph.html         # Interactive vis.js viewer — open in browser
├── graph.svg          # Static vector image
├── graph.cypher       # Neo4j import script
├── obsidian/          # Obsidian vault (35 .md files with wikilinks)
└── wiki/              # Agent-crawlable wiki (index + community pages)
```

## Results

**6 C# files → 47 nodes, 79 edges, 7 communities detected.**

This example runs in AST-only mode (100% EXTRACTED confidence) — no AI provider needed. It demonstrates the full pipeline: file detection, AST extraction, graph building, Louvain clustering, analysis, and all 7 export formats.

## Export Format Details

For details on each output format, see:

- [Export Formats Overview](export-formats.md)
- [HTML Interactive Viewer](format-html.md)
- [JSON Graph Export](format-json.md)
- [SVG Graph Export](format-svg.md)
- [Neo4j Cypher Export](format-neo4j.md)
- [Obsidian Vault Export](format-obsidian.md)
- [Wiki Export](format-wiki.md)
- [Graph Analysis Report](format-report.md)
