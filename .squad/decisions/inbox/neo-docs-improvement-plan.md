# Documentation Improvement Plan

**Author:** Neo (Lead/Architect)  
**Date:** 2026-04-07  
**Status:** Proposed — For Trinity to implement  
**Scope:** All 19 docs + README.md + ARCHITECTURE.md

---

## Executive Summary

I read every doc as a brand-new user. The README gets you oriented in ~30 seconds — that's good. But the docs have a structural problem: **they describe features exhaustively but never walk you through actually using them**. There are 19 docs totaling ~3,500 lines. A new user drowns in reference material without a single guided path from install to "wow, look at this graph."

The Python original (safishamsi/graphify) ships *worked examples with real output* — three corpora of different sizes with actual GRAPH_REPORT.md and graph.json committed. Our worked-example.md is 48 lines that say "run this command" and "look at the output." No screenshots, no walkthrough of what to look at, no interpretation of results.

---

## Ranked Improvements (Highest Impact First)

### 1. CREATE: `docs/getting-started.md` — A Real Tutorial (not Quick Start)

**Impact:** CRITICAL — This is the single biggest gap  
**File:** `docs/getting-started.md` (new)  
**Why:** The README "Quick Start" is 3 commands. That's a reference snippet, not a tutorial. A new user needs a guided 5-minute walkthrough:

1. Install the tool (with verification that it works)
2. Point it at a real directory (the `samples/mini-library/`)
3. See the output files appear
4. Open `graph.html` — what are they looking at? What do the colors mean?
5. Open `GRAPH_REPORT.md` — how to read the god nodes, communities, suggestions
6. "Now try it on YOUR code" call-to-action

**Content:** ~200 lines. Step-by-step with expected terminal output at each step. Link to the worked example for deeper analysis. This should be the #1 link in the README docs table, above Architecture.

**Cross-link:** README Quick Start should say "For a guided walkthrough, see [Getting Started](docs/getting-started.md)."

---

### 2. REWRITE: `docs/worked-example.md` — Add Real Walkthrough with Output Samples

**Impact:** HIGH — The Python version's worked examples are its trust-building mechanism  
**File:** `docs/worked-example.md` (rewrite)  
**Why:** Current version is 48 lines: "run this command, here's the file tree." The Python original commits full output (GRAPH_REPORT.md, graph.json) for three different corpora so users can verify results without running anything. Our `samples/mini-library/graphify-out/` has real output — but the doc never shows it.

**Content needed:**
- Inline a summary of `GRAPH_REPORT.md` output (the god nodes, communities)
- Show a snippet of `graph.json` structure with explanation
- Describe what you see when you open `graph.html` (which nodes, which clusters, what to click)
- Show the Obsidian vault structure and explain wikilinks
- Describe the Neo4j Cypher import flow
- Add a "What to look for" section interpreting the 47-node / 79-edge / 7-community result
- Note: the GRAPH_REPORT shows `MiniLibrary` appearing 5 times in god nodes — this is a real data quality issue worth calling out as an example of graph interpretation

**Stretch:** Add a second worked example with a larger or more interesting corpus (like the tool itself — `graphify run src/Graphify`).

---

### 3. CREATE: `docs/troubleshooting.md` — FAQ and Common Problems

**Impact:** HIGH — Every new tool needs this; only `dotnet-tool-install.md` has a troubleshooting section  
**File:** `docs/troubleshooting.md` (new)  
**Why:** Scattered troubleshooting exists in `dotnet-tool-install.md`, `setup-azure-openai.md`, `setup-ollama.md`, and `setup-copilot-sdk.md` — but there's no central page. Common questions a new user will have:

- "I ran `graphify run .` and got no output / empty graph" (probably no supported files found)
- "The graph.html is empty / has only a few nodes" (check `--verbose` flag)
- "How do I use this without an AI provider?" (AST-only mode — mentioned in passing in README but not explained anywhere)
- "What file types are supported?" (scattered across docs, never listed in one place)
- "My graph has duplicate/weird nodes" (known issue visible in worked example output)
- "Watch mode isn't detecting changes" (filesystem watcher limitations)
- ".NET 10 SDK not found" (already in dotnet-tool-install.md)

**Content:** ~150 lines. Organize as problem → cause → solution. Link to relevant detailed docs.

---

### 4. ADD TO README: Supported File Types and AST-Only Mode

**Impact:** HIGH — Critical information that's missing from the most-read page  
**File:** `README.md` (edit)  
**Why:** The README never says what file types are supported. The blog post says "Python, TypeScript, JavaScript, Go, Rust, Java, C#, C++, and more" but the README is silent. Also, AST-only mode (no AI provider needed) is mentioned once in the Quick Start config wizard but never explained. A user who doesn't have Azure OpenAI / Ollama / Copilot needs to know they can still get value.

**Content:** Add a short "Supported Languages" section after Quick Start listing supported file types. Add one sentence: "No AI provider? No problem — AST-only extraction works with zero config and produces structural graphs from code."

---

### 5. FIX: Inconsistencies Between Docs

**Impact:** MEDIUM — Confusing but not blocking  
**Files:** Multiple

**Specific inconsistencies found:**

| Issue | Location | Fix |
|-------|----------|-----|
| Default formats: README says `json,html,report`; `dotnet-tool-install.md` says `json,html`; `cli-reference.md` says `json,html,report` | README, dotnet-tool-install.md, cli-reference.md | Pick one truth, update all three |
| Blog post shows `query` and `explain` and `export` subcommands that don't exist in CLI reference | `docs/blog-post.md` | Fix commands to match actual CLI (`run`, `watch`, `benchmark`, `config`) |
| Blog post says "GraphML" as an export format; actual formats don't include GraphML | `docs/blog-post.md` | Fix to match actual formats |
| `setup-azure-openai.md` and `setup-ollama.md` link to `BrunoCapuano/graphify-dotnet`; `setup-copilot-sdk.md` links to `elbruno/graphify-dotnet` | Provider setup docs | Normalize all to `elbruno/graphify-dotnet` |
| `setup-azure-openai.md` shows `AZURE_OPENAI_*` env vars but `configuration.md` shows `GRAPHIFY__AzureOpenAI__*` | azure setup, configuration | Clarify: `GRAPHIFY__*` is the app config system; `AZURE_OPENAI_*` is for the code example |
| `dotnet-tool-install.md` common options table omits `copilotsdk` from `--provider` values | dotnet-tool-install.md | Add `copilotsdk` to provider list |
| `format-obsidian.md` mentions `--filter "community:Auth"` — this flag doesn't appear in CLI reference | format-obsidian.md | Either add the flag to CLI or remove from docs |
| Ollama code examples use `OllamaOptions` constructor (non-existent) alongside `AiProviderOptions` | setup-ollama.md | Normalize all to `AiProviderOptions` |
| `image-prompts.md` links to `.github/copilot-instructions.md` without `../` prefix | image-prompts.md | Fix relative path |

---

### 6. ADD: Screenshots / Visual Examples of Output

**Impact:** MEDIUM — "Show, don't tell" is the #1 docs principle  
**Files:** `docs/worked-example.md`, `docs/format-html.md`, `docs/format-obsidian.md`, `docs/format-svg.md`  
**Why:** The Python version's power comes from seeing the visual output. We have real output in `samples/mini-library/graphify-out/` including an actual `graph.html` and `graph.svg`. We should:

1. Commit a screenshot of `graph.html` in the browser (PNG, ~200KB)
2. Embed the SVG directly in the worked example doc (`![Graph](../samples/mini-library/graphify-out/graph.svg)`)
3. Show a screenshot of the Obsidian vault opened in Obsidian
4. Add these to the relevant format docs

**Note:** `image-prompts.md` has AI image generation prompts for marketing images. Those are nice but not the same as actual screenshots of real output. Users want to see REAL output, not conceptual art.

---

### 7. REWRITE: `docs/export-formats.md` — Remove Redundancy with Individual Format Docs

**Impact:** MEDIUM — 332 lines that largely duplicate the 7 individual format docs  
**File:** `docs/export-formats.md` (trim)  
**Why:** This doc repeats content from `format-html.md`, `format-json.md`, etc. It's useful as a comparison page but the "Format Details" section (lines 148-278) is redundant. Keep the comparison tables and workflow sections; remove the duplicate format descriptions and replace with links.

---

### 8. ADD: Cross-Links from Format Docs Back to Worked Example

**Impact:** LOW-MEDIUM — Improves navigation  
**Files:** All `format-*.md` files  
**Why:** Every format doc says "here's what it produces" with example content — but none of them say "see the worked example for real output." Add a line like: "See [Worked Example](worked-example.md) for actual output from a real C# project."

---

### 9. CLARIFY: `docs/configuration.md` — Which Env Var Prefix Is Correct?

**Impact:** MEDIUM — Will confuse users who try both  
**File:** `docs/configuration.md` (edit)  
**Why:** Configuration doc shows `GRAPHIFY__Provider=AzureOpenAI` but `setup-azure-openai.md` shows both `AZURE_OPENAI_ENDPOINT` (for code examples) and `GRAPHIFY__AzureOpenAI__Endpoint` (for CLI config). Need a clear statement: "The CLI uses `GRAPHIFY__*` prefix. The `AZURE_OPENAI_*` variables in code examples are for programmatic SDK usage."

---

### 10. TRIM: `docs/future-plans.md` — Move to a Wiki or Reduce Scope

**Impact:** LOW — 21KB internal planning doc shouldn't be in user-facing docs  
**File:** `docs/future-plans.md` (move or trim)  
**Why:** This is a 250+ line internal roadmap document with ecosystem analysis, difficulty ratings, and implementation notes. It's valuable for the team but confusing for a new user browsing `/docs/`. Either:
- Move to `.squad/` or a `ROADMAP.md` at root (standard OSS pattern)
- Or trim to a short "What's Coming" section with 5-8 bullet points

---

### 11. TRIM: `docs/image-prompts.md` — Not User-Facing Documentation

**Impact:** LOW — Internal marketing asset in the docs folder  
**File:** `docs/image-prompts.md` (move)  
**Why:** AI image generation prompts are not documentation. Move to `.squad/` or a `marketing/` folder. A new user browsing docs shouldn't encounter this.

---

### 12. ADD: `docs/mcp-server.md` — MCP Integration Doc

**Impact:** LOW-MEDIUM — Feature exists in codebase but has zero documentation  
**File:** `docs/mcp-server.md` (new)  
**Why:** The README mentions MCP server in the project structure. The future-plans doc references MCP operations (query, explain, path, communities, analyze). But there is no doc explaining what MCP is, how to start the server, or how to connect it to Claude/Copilot. Even a stub doc would be better than silence.

---

### 13. ADD TO README: Documentation Table Improvements

**Impact:** LOW — Better navigation  
**File:** `README.md` (edit)  
**Why:** The docs table in README is good but missing entries for:
- Getting Started (once created)
- Troubleshooting (once created)
- MCP Server (once created)
- Future Plans / Roadmap

Also, the blog post and image prompts shouldn't be in the docs table (they're not user docs).

---

## Docs That Are Good As-Is (Minor Tweaks Only)

These docs are well-written and comprehensive. Just need the cross-link and consistency fixes noted above:

- **`cli-reference.md`** — Excellent. Complete, well-organized, good examples.
- **`configuration.md`** — Good layered config explanation. Needs env var clarification (item #9).
- **`dotnet-tool-install.md`** — Best doc in the set. Has real troubleshooting, clear steps, multiple methods.
- **`setup-azure-openai.md`** — Thorough. Fix GitHub repo link inconsistency.
- **`setup-ollama.md`** — Thorough. Fix `OllamaOptions` code example.
- **`setup-copilot-sdk.md`** — Good. Fix `Create` vs `CreateAsync` is well-documented.
- **`watch-mode.md`** — Excellent. Programmatic usage, IDE integration, clear limitations.
- **`format-json.md`** — Good schema docs with multi-language examples.
- **`format-neo4j.md`** — Good. Cypher examples are practical.

## Docs That Need Rewriting

- **`worked-example.md`** — Near-total rewrite (item #2)
- **`export-formats.md`** — Significant trimming (item #7)
- **`blog-post.md`** — Fix incorrect CLI commands (item #5)

## Docs That Should Move Out of `/docs/`

- **`future-plans.md`** → `ROADMAP.md` at root or `.squad/`
- **`image-prompts.md`** → `.squad/` or `marketing/`
- **`blog-post.md`** → Could stay, but should be clearly labeled as marketing content

---

## Priority Implementation Order

1. Create `docs/getting-started.md` (unblocks new users)
2. Rewrite `docs/worked-example.md` (trust-building)
3. Fix all inconsistencies (table in item #5)
4. Create `docs/troubleshooting.md`
5. Add supported file types to README
6. Add screenshots/SVG to worked example and format docs
7. Add cross-links between all format docs and worked example
8. Trim export-formats.md redundancy
9. Create `docs/mcp-server.md` stub
10. Move internal docs out of `/docs/`
