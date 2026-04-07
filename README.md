# graphify-dotnet

[![CI Build](https://github.com/elbruno/graphify-dotnet/actions/workflows/build.yml/badge.svg)](https://github.com/elbruno/graphify-dotnet/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub stars](https://img.shields.io/github/stars/elbruno/graphify-dotnet?style=social)](https://github.com/elbruno/graphify-dotnet/stargazers)

🔍 **Build AI-powered knowledge graphs from any codebase.** Understand structure you didn't know was there.

> 💡 **Origin story** — This project traces back to [Andrej Karpathy's tweet](https://x.com/karpathy/status/2039805659525644595) on using LLMs to build personal knowledge bases: ingesting raw sources, compiling them into structured Markdown wikis, and navigating knowledge through graph views instead of keyword search. That idea inspired [graphify](https://github.com/safishamsi/graphify) by [@safishamsi](https://github.com/safishamsi), which was then [showcased by @socialwithaayan](https://x.com/socialwithaayan/status/2041192946369007924) — and that's what kicked off this .NET port.

graphify-dotnet reads your files (code, docs, images), extracts concepts and relationships through AST parsing and AI semantic analysis, builds a knowledge graph with community detection, and exports interactive visualizations. Navigate codebases by structure instead of keyword search.

## Quick Start

### 1. Install

```bash
dotnet tool install -g graphify-dotnet
```

### 2. Configure

```bash
graphify config
```

This launches an interactive wizard with three options:

- **📋 View current configuration** — see what's set
- **🔧 Set up AI provider** — pick Azure OpenAI, Ollama, Copilot SDK, or None (AST-only)
- **📂 Set folder to analyze** — set your default project folder and export formats

Pick your AI provider (or skip it — AST-only extraction works with zero config), then set the folder to analyze.

### 3. Run

```bash
graphify run
```

That's it. Open `graphify-out/graph.html` in your browser to explore the interactive graph.

## Build from Source

```bash
git clone https://github.com/elbruno/graphify-dotnet.git
cd graphify-dotnet
dotnet build graphify-dotnet.slnx
dotnet run --project src/Graphify.Cli -- run .
```

## Documentation

| Topic | Link |
|-------|------|
| **Architecture** | [ARCHITECTURE.md](ARCHITECTURE.md) |
| **Configuration** | [docs/configuration.md](docs/configuration.md) |
| **CLI Reference** | [docs/cli-reference.md](docs/cli-reference.md) |
| **Worked Example** | [docs/worked-example.md](docs/worked-example.md) |
| | |
| **AI Providers** | |
| &ensp; Azure OpenAI | [docs/setup-azure-openai.md](docs/setup-azure-openai.md) |
| &ensp; Ollama | [docs/setup-ollama.md](docs/setup-ollama.md) |
| &ensp; Copilot SDK | [docs/setup-copilot-sdk.md](docs/setup-copilot-sdk.md) |
| | |
| **Export Formats** | [docs/export-formats.md](docs/export-formats.md) |
| &ensp; HTML Interactive | [docs/format-html.md](docs/format-html.md) |
| &ensp; JSON | [docs/format-json.md](docs/format-json.md) |
| &ensp; SVG | [docs/format-svg.md](docs/format-svg.md) |
| &ensp; Neo4j Cypher | [docs/format-neo4j.md](docs/format-neo4j.md) |
| &ensp; Obsidian Vault | [docs/format-obsidian.md](docs/format-obsidian.md) |
| &ensp; Wiki | [docs/format-wiki.md](docs/format-wiki.md) |
| &ensp; Report | [docs/format-report.md](docs/format-report.md) |
| | |
| **Other** | |
| &ensp; Watch Mode | [docs/watch-mode.md](docs/watch-mode.md) |
| &ensp; Global Tool Install | [docs/dotnet-tool-install.md](docs/dotnet-tool-install.md) |

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) file for details.

## 👋 About the Author

**Made with ❤️ by [Bruno Capuano (ElBruno)](https://github.com/elbruno)**

- 📝 **Blog**: [elbruno.com](https://elbruno.com)
- 📺 **YouTube**: [youtube.com/elbruno](https://youtube.com/elbruno)
- 🔗 **LinkedIn**: [linkedin.com/in/elbruno](https://linkedin.com/in/elbruno)
- 𝕏 **Twitter**: [twitter.com/elbruno](https://twitter.com/elbruno)
- 🎙️ **Podcast**: [notienenombre.com](https://notienenombre.com)

## Acknowledgments

- [Andrej Karpathy's tweet](https://x.com/karpathy/status/2039805659525644595) on LLM-powered personal knowledge bases — the original idea that started the chain.
- [This tweet](https://x.com/socialwithaayan/status/2041192946369007924) by @socialwithaayan showcasing [graphify](https://github.com/safishamsi/graphify) by @safishamsi — which directly inspired this .NET port.

This project is a .NET 10 port of [safishamsi/graphify](https://github.com/safishamsi/graphify), reimagined with C# idioms, .NET 10 features, and the Microsoft.Extensions.AI abstraction layer.
