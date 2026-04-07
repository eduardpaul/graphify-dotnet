# Watch Mode — Incremental Knowledge Graph Updates

Watch Mode monitors your codebase for file changes and incrementally updates the knowledge graph in real-time, avoiding expensive full rebuilds for large projects.

## Quick Start

```bash
# Start watching with default output directory
graphify watch . --output ./graph-out

# Watch with verbose output
graphify watch ./src --output ./reports --verbose

# Stop with Ctrl+C
```

Once started, any file changes are detected within 500ms and the graph is updated automatically.

## What It Does

Watch Mode follows this workflow:

1. **Initial Build** — Runs the full pipeline (file detection, extraction, graph building, clustering, export)
2. **Change Detection** — Monitors the directory tree using `FileSystemWatcher`
3. **Debouncing** — Waits 500ms to batch rapid changes (e.g., IDE auto-save)
4. **Content Verification** — Uses SHA256 hashes to verify files actually changed (not just timestamps)
5. **Selective Re-extraction** — Re-extracts only the changed files
6. **Incremental Merge** — Merges new extraction results into the existing graph
7. **Re-clustering** — Updates community detection and relationships
8. **Export** — Writes updated graph to JSON, HTML, or other formats

This approach is ideal for large codebases where full rebuilds (5-30 seconds) would slow development.

## CLI Usage

### Basic Command

```bash
graphify watch <path> [options]
```

### Options

| Option | Short | Default | Description |
|--------|-------|---------|-------------|
| `--output` | `-o` | `graphify-out` | Output directory for graph files |
| `--format` | `-f` | `json,html` | Export formats (comma-separated) |
| `--verbose` | `-v` | — | Detailed logging |

### Examples

```bash
# Watch current directory with defaults
graphify watch .

# Watch specific subdirectory
graphify watch ./src

# Custom output location
graphify watch . --output ./analysis

# JSON only (faster)
graphify watch . --format json

# All options combined
graphify watch ./src --output ./reports --format json,html --verbose

# Watch with verbose output to see change details
graphify watch . --verbose
```

## Programmatic Usage with C#

You can use Watch Mode directly in your C# code:

```csharp
using Graphify.Pipeline;

// Create a watch mode instance
var watchMode = new WatchMode(
    output: Console.Out,
    verbose: true
);

// Run the initial pipeline first
var runner = new PipelineRunner(Console.Out, verbose: true);
var initialGraph = await runner.RunAsync(
    path: "./src",
    output: "./graph-out",
    formats: new[] { "json", "html" },
    useCache: true,
    CancellationToken.None
);

// Set the initial graph
watchMode.SetInitialGraph(initialGraph);

// Start watching (runs until cancellation)
using var cts = new CancellationTokenSource();

// Optional: handle Ctrl+C
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Watch for changes
await watchMode.WatchAsync(
    path: "./src",
    output: "./graph-out",
    formats: new[] { "json", "html" },
    ct: cts.Token
);
```

## How It Works Under the Hood

### File Change Detection

Watch Mode ignores common non-essential directories:
- `.git`, `.gitignore`, `.vscode`, and other hidden directories
- `bin/` and `obj/` (build output)
- Cache and temporary files

Changes are tracked by file path, with rapid changes batched and debounced over 500ms.

### Content Verification

Not every file system event means the file content changed. Watch Mode uses **SHA256 hashing** to verify:

```csharp
// Only re-process if content truly changed
if (await cache.IsChangedAsync(filePath))
{
    // File content is different → re-extract
}
```

This prevents unnecessary re-processing when:
- File timestamps are updated without content change
- IDEs rewrite files with identical content
- File explorers touch the file

### Incremental Graph Update

When files change:

1. **Merge** — New extraction results are merged into the existing graph using `MostRecent` strategy
2. **Re-cluster** — Community detection runs on the merged graph to update relationships
3. **Re-export** — Updated graph is written to all configured formats

The entire process typically takes 500ms–2s depending on the change size.

## Performance Notes

### When to Use Watch Mode

- **Development workflow** — Monitor changes while writing code
- **Large codebases** — 1000+ files where full rebuilds are expensive
- **CI/CD pipelines** — Continuous graph updates as code is committed
- **IDE integration** — Real-time knowledge graph updates

### Typical Performance

| Scenario | Time |
|----------|------|
| Change 1–5 files | 500ms–1s |
| Change 10–20 files | 1–3s |
| Change 50+ files | 3–10s |
| Full rebuild (no watch) | 5–30s (depends on project size) |

### Memory Usage

Watch Mode keeps the graph in memory for fast incremental updates. For very large graphs (10k+ nodes), consider:
- Watching specific subdirectories (`graphify watch ./src`)
- Using file filters (via `.gitignore`)

## Limitations

### Known Limitations

1. **Deleted files** — Watch Mode detects when files are deleted but doesn't remove stale references from the graph. Nodes/edges from deleted files remain until a full rebuild.
   - *Workaround:* Run `graphify run .` periodically to clean up stale references.

2. **Rename detection** — File renames are detected as delete + create, which may create duplicate nodes temporarily until the graph is rebuilt.

3. **Untracked changes** — Changes outside the watch path (e.g., external files, imports) are not automatically detected.

These limitations are planned improvements for future versions.

## Output Files

By default, Watch Mode exports:

- **`graph.json`** — Machine-readable graph (nodes and edges)
- **`graph.html`** — Interactive graph visualization

Both are updated in real-time as the graph is modified.

## Use Cases

### Development Workflow

Monitor your codebase as you code:

```bash
# Terminal 1: Watch your code
graphify watch ./src --output ./graph-out --verbose

# Terminal 2 (or IDE): Make changes and see graph updates live
```

### CI/CD Pipeline

Maintain a live graph of your repository:

```bash
# In your CI/CD job
graphify watch . --output ./public/graph --format json,html
```

The graph updates incrementally as pull requests are merged.

### IDE Integration

Use a task or extension to watch your project:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Graphify Watch",
      "type": "shell",
      "command": "graphify watch . --output ./graph-out",
      "isBackground": true,
      "problemMatcher": {
        "pattern": {
          "regexp": "^.*$",
          "file": 1,
          "location": 2,
          "message": 3
        },
        "background": {
          "activeOnStart": true,
          "beginsPattern": "^Watching.*",
          "endsPattern": "^.*"
        }
      }
    }
  ]
}
```

## Stopping Watch Mode

Press **Ctrl+C** to gracefully stop the watch:

```
[14:23:45] Change detected in 3 file(s):
  ~ src/Services/UserService.cs
  ~ src/Models/User.cs
  ~ tests/UserServiceTests.cs
  Re-processed 3 file(s) → 1,234 nodes, 5,678 edges

  Export to ./graph-out
^C
Watch stopped.
```

## See Also

- [Installing as a Global Tool](./dotnet-tool-install.md) — Setup guide
- [README](../README.md) — Project overview
