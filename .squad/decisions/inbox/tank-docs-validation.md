# Documentation Validation Report

**Author:** Tank (Tester)
**Date:** 2026-04-07
**Method:** Compared every doc code example, command, and class name against actual source code, CLI help output, and sample data.

---

## 1. README.md

| Item | Status | Details |
|------|--------|---------|
| `dotnet tool install -g graphify-dotnet` | ⚠️ | Package is NOT published to NuGet.org. The csproj has `PackAsTool=true` and correct metadata, but a new user will get a "not found" error. Should note "coming soon" or link to build-from-source. |
| `graphify config` | ✅ | Command exists in Program.cs with interactive Spectre.Console wizard and 3 menu options as described. |
| `graphify run` | ✅ | Command exists with all described options. |
| `dotnet build graphify-dotnet.slnx` | ✅ | Works. |
| `dotnet run --project src/Graphify.Cli -- run .` | ✅ | Works. |
| `graphify-out/graph.html` default output path | ✅ | Correct — default output dir is `graphify-out`. |
| Doc table links | ✅ | All linked files exist: ARCHITECTURE.md, docs/configuration.md, docs/cli-reference.md, etc. |

---

## 2. docs/cli-reference.md

| Item | Status | Details |
|------|--------|---------|
| `run` command exists | ✅ | Confirmed in Program.cs and --help output. |
| `watch` command exists | ✅ | Confirmed. |
| `benchmark` command exists | ✅ | Confirmed. |
| `config` command + subcommands (show, set, folder) | ✅ | All confirmed in Program.cs. |
| `run` flags: --output/-o, --format/-f, --verbose/-v, --provider/-p, --endpoint, --api-key, --model, --deployment, --config/-c | ✅ | All match Program.cs exactly. |
| `run` default format: `json,html,report` | ✅ | Matches Program.cs line 34. |
| Building from source commands | ✅ | Correct. |

---

## 3. docs/configuration.md

| Item | Status | Details |
|------|--------|---------|
| Interactive wizard with 3 options | ✅ | Matches Program.cs lines 256-263. |
| Config subcommands (show, set, folder) | ✅ | All exist. |
| Config file path: `appsettings.local.json` | ✅ | Confirmed in ConfigPersistence.cs. |
| **Priority order** | ❌ | **WRONG.** Doc says: 1. CLI args, 2. Env vars, 3. User secrets, 4. appsettings.local.json, 5. appsettings.json. **Actual code** (ConfigurationFactory.cs): 1. CLI args, 2. User secrets, 3. Env vars, 4. appsettings.local.json, 5. appsettings.json. User secrets and env vars are **swapped** — user secrets actually override env vars. |
| Env var prefix `GRAPHIFY__` | ✅ | Correct via `.AddEnvironmentVariables()`. |
| User secrets examples | ✅ | Key names match GraphifyConfig.cs. |

---

## 4. docs/setup-ollama.md

| Item | Status | Details |
|------|--------|---------|
| `OllamaClientFactory` class exists | ✅ | At src/Graphify.Sdk/OllamaClientFactory.cs. |
| `ChatClientFactory.Create(AiProviderOptions)` | ✅ | Exists, dispatches to OllamaClientFactory. |
| `AiProviderOptions` record fields | ✅ | Provider, Endpoint, ModelId all correct. |
| `OllamaOptions` record | ✅ | Exists with Endpoint and ModelId params. |
| CLI syntax `--provider ollama --model codellama` | ✅ | Correct. |
| **Configuration priority order** | ❌ | Same bug as configuration.md — env vars and user secrets swapped. Also **missing appsettings.local.json** from the list (only shows 4 layers instead of 5). |
| **Programmatic code: `client.CompleteAsync()` / `response.Message`** | ❌ | **WRONG API.** The project uses M.E.AI 10.x which uses `client.GetResponseAsync()` returning `ChatResponse` with `.Text`. The `CompleteAsync`/`.Message` pattern is the old 9.x API. Source code confirms: SemanticExtractor.cs and CopilotChatClient.cs both use `GetResponseAsync`. |
| "Development" section: `OllamaClientFactory.Create(options)` | ✅ | Code is correct — OllamaClientFactory.Create(OllamaOptions) exists. |
| **"Need help?" URL** | ❌ | Links to `github.com/BrunoCapuano/graphify-dotnet`. Should be `github.com/elbruno/graphify-dotnet` (per README badges and csproj RepositoryUrl). |
| API ref link: `../src/Graphify.Sdk/OllamaClientFactory.cs` | ✅ | File exists. |

---

## 5. docs/setup-azure-openai.md

| Item | Status | Details |
|------|--------|---------|
| `AzureOpenAIClientFactory` class exists | ✅ | At src/Graphify.Sdk/AzureOpenAIClientFactory.cs. |
| `AzureOpenAIOptions` record fields (Endpoint, ApiKey, DeploymentName, ModelId) | ✅ | Matches source. |
| `ChatClientFactory.Create(AiProviderOptions)` with AzureOpenAI | ✅ | Correctly dispatches to AzureOpenAIClientFactory. |
| CLI syntax with `--provider azureopenai --endpoint --api-key --deployment` | ✅ | All correct. |
| **Configuration priority order** | ❌ | Same bug — env vars and user secrets swapped, and **missing appsettings.local.json** (only 4 layers shown). |
| **Programmatic code: `client.CompleteAsync()` / `response.Message`** | ❌ | **WRONG API.** Should be `client.GetResponseAsync([new ChatMessage(...)])` / `response.Text`. |
| **"Need help?" URL** | ❌ | Links to `github.com/BrunoCapuano/graphify-dotnet`. Should be `github.com/elbruno/graphify-dotnet`. |
| API ref link: `../src/Graphify.Sdk/AzureOpenAIClientFactory.cs` | ✅ | File exists. |

---

## 6. docs/setup-copilot-sdk.md

| Item | Status | Details |
|------|--------|---------|
| `CopilotSdkClientFactory` class exists | ✅ | At src/Graphify.Sdk/CopilotSdkClientFactory.cs. |
| `CopilotChatClient` class exists | ✅ | At src/Graphify.Sdk/CopilotChatClient.cs. |
| `CopilotSdkOptions` record with ModelId | ✅ | Matches source. Default "gpt-4.1" correct. |
| `ChatClientFactory.CreateAsync()` for CopilotSdk | ✅ | Correctly requires async, throws on sync Create(). |
| `CopilotClient` with `UseLoggedInUser = true` | ✅ | Matches CopilotSdkClientFactory.cs. |
| **Programmatic code: `client.GetResponseAsync()` / `response.Text`** | ✅ | **Correct!** Uses the right M.E.AI 10.x API. |
| Microsoft Agent Framework packages | ✅ | `Microsoft.Agents.AI` and `Microsoft.Agents.AI.GitHub.Copilot` are in Graphify.Sdk.csproj. |
| **Configuration priority order** | ❌ | Same bug — env vars and user secrets swapped, missing appsettings.local.json. |
| "Need help?" URL | ✅ | Links to `github.com/elbruno/graphify-dotnet` — correct. |

---

## 7. docs/worked-example.md

| Item | Status | Details |
|------|--------|---------|
| `samples/mini-library/` exists | ✅ | Directory exists with src/ and graphify-out/. |
| Pre-generated output exists | ✅ | GRAPH_REPORT.md, graph.json, graph.html, graph.svg, graph.cypher, obsidian/, wiki/ all present. |
| **"6 C# files"** | ❌ | **WRONG.** There are **5** .cs files (IRepository.cs, ServiceCollectionExtensions.cs, User.cs, UserRepository.cs, UserService.cs). |
| "47 nodes, 79 edges" | ✅ | Confirmed from graph.json. |
| "7 communities detected" | ✅ | Confirmed — community IDs 0-6 in graph.json. |
| **"obsidian/ (35 .md files with wikilinks)"** | ❌ | **WRONG.** There are **30** .md files in obsidian/. |
| Output tree structure | ✅ | All listed files/dirs match actual output. |

---

## 8. docs/export-formats.md

| Item | Status | Details |
|------|--------|---------|
| 7 formats listed | ✅ | json, html, svg, neo4j, obsidian, wiki, report — all have corresponding exporters in src/Graphify/Export/. |
| Output file names | ✅ | graph.json, graph.html, graph.svg, graph.cypher, obsidian/, wiki/, GRAPH_REPORT.md — all match PipelineRunner.cs. |
| Default formats: `json,html,report` | ✅ | Matches Program.cs. |

---

## 9. docs/format-json.md

| Item | Status | Details |
|------|--------|---------|
| Output file: `graph.json` | ✅ | Correct. |
| **JSON schema — top-level `communities` array** | ❌ | **WRONG.** The actual JSON has only `nodes`, `edges`, `metadata`. There is NO top-level `communities` array. Communities are stored as `community` field on individual nodes. |
| **Metadata field names** | ❌ | **WRONG.** Doc shows `timestamp`, `version`, `source`, `nodeCount`, `edgeCount`, `communityCount` (camelCase). Actual metadata uses `node_count`, `edge_count`, `community_count`, `generated_at` (snake_case). Also `version` and `source` fields don't exist. |
| **Node field `degree`** | ❌ | **WRONG.** The actual JSON node DTO does not include a `degree` field. Actual fields: id, label, type, community, file_path, language, confidence, metadata. |
| **Node field `description`** | ❌ | **WRONG.** No `description` field in node output. |
| **Edge field `extractionMethod`** | ❌ | **WRONG.** No `extractionMethod` field in edge output. Actual fields: source, target, relationship, weight, confidence, metadata. |

---

## 10. docs/format-html.md, format-svg.md, format-neo4j.md, format-obsidian.md, format-wiki.md, format-report.md

| Item | Status | Details |
|------|--------|---------|
| Exporter classes exist | ✅ | HtmlExporter, SvgExporter, Neo4jExporter, ObsidianExporter, WikiExporter, ReportGenerator — all found in src/Graphify/Export/. |
| Output file names match | ✅ | graph.html, graph.svg, graph.cypher, obsidian/, wiki/, GRAPH_REPORT.md — all match PipelineRunner.cs. |
| format-html.md: vis-network mention | ✅ | HtmlTemplate.cs exists in Export/ (vis-network based). |
| General format descriptions | ✅ | Descriptions are accurate based on what the exporters produce. |

---

## 11. docs/watch-mode.md

| Item | Status | Details |
|------|--------|---------|
| `graphify watch` command | ✅ | Exists in Program.cs. |
| WatchMode class in `Graphify.Pipeline` | ✅ | At src/Graphify/Pipeline/WatchMode.cs. |
| Constructor `WatchMode(TextWriter, bool)` | ✅ | Matches source. |
| `SetInitialGraph()` method | ✅ | Exists in WatchMode.cs. |
| `WatchAsync()` method | ✅ | Exists. |
| FileSystemWatcher + 500ms debounce + SHA256 | ✅ | Confirmed in source. |
| **Default format for watch: `json,html`** | ❌ | **WRONG.** Watch uses the same `AddPipelineOptions` as run, which defaults to `json,html,report`. |
| **Programmatic example: `new PipelineRunner(Console.Out, verbose: true)`** | ❌ | **WRONG namespace.** Doc shows `using Graphify.Pipeline;` but PipelineRunner is in `Graphify.Cli` namespace. Should be `using Graphify.Cli;`. Also, PipelineRunner constructor requires 3 params: `(TextWriter, bool, IChatClient?)`. |

---

## 12. docs/dotnet-tool-install.md

| Item | Status | Details |
|------|--------|---------|
| Tool metadata in csproj | ✅ | PackAsTool=true, ToolCommandName=graphify, PackageId=graphify-dotnet — all correct. |
| ⚠️ "dotnet tool install -g graphify-dotnet" from NuGet | ⚠️ | Not yet published. Doc line 37 says "Once graphify-dotnet is published to NuGet.org" which is honest, but the Quick Start at line 8 shows it without qualifier. |
| Local build installation | ✅ | `dotnet pack` + `--add-source` approach is correct. |
| **`--format` default: `json,html`** | ❌ | **WRONG.** Line 124 says default is `json,html`. Actual default is `json,html,report`. |
| **`--provider` short form missing** | ⚠️ | Line 125 shows `--provider` without `-p` short alias. The actual CLI has `-p` alias. Minor omission. |

---

## Summary of Issues

### ❌ Critical (Wrong Information)

1. **Config priority order** — User secrets and environment variables are swapped in docs/configuration.md, setup-ollama.md, setup-azure-openai.md, setup-copilot-sdk.md. Actual: user secrets > env vars (user secrets win).
2. **`CompleteAsync`/`.Message` API** — setup-ollama.md and setup-azure-openai.md use deprecated M.E.AI 9.x API. Should be `GetResponseAsync`/`.Text` (M.E.AI 10.x). setup-copilot-sdk.md is correct.
3. **format-json.md JSON schema** — Multiple wrong fields: `communities` array doesn't exist; metadata fields use wrong names/casing; `degree`, `description`, `extractionMethod` fields don't exist in output.
4. **worked-example.md file count** — Says "6 C# files" but there are 5. Says "35 .md files" in obsidian but there are 30.
5. **watch-mode.md PipelineRunner namespace** — Shows `using Graphify.Pipeline` but PipelineRunner is in `Graphify.Cli`.
6. **GitHub URL** — setup-ollama.md and setup-azure-openai.md link to `BrunoCapuano/graphify-dotnet` instead of `elbruno/graphify-dotnet`.

### ⚠️ Misleading

7. **`--format` default** — dotnet-tool-install.md and watch-mode.md say `json,html` but actual default is `json,html,report`.
8. **NuGet install** — README.md and dotnet-tool-install.md Quick Start suggest `dotnet tool install -g graphify-dotnet` as if it works today. Not published yet.
9. **Missing appsettings.local.json** — setup-ollama.md, setup-azure-openai.md, setup-copilot-sdk.md only show 4 config layers instead of 5 (missing appsettings.local.json).
