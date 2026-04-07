# Troubleshooting

Common problems and solutions. Organized as problem → cause → fix.

---

## "graphify: command not found"

**Cause:** The global tool isn't on your PATH yet, or it isn't installed.

**Fix:**

1. Check if it's installed:
   ```bash
   dotnet tool list -g | grep graphify
   ```

2. If not listed, install it:
   ```bash
   dotnet tool install -g graphify-dotnet
   ```

3. If listed but not found, restart your terminal. The PATH update only applies to new sessions.
   - **PowerShell:** Open a new window, or run `$env:PATH = [System.Environment]::GetEnvironmentVariable("PATH", "User")`
   - **Bash/Zsh:** Run `source ~/.bashrc` or `source ~/.zshrc`

See [Global Tool Install](dotnet-tool-install.md) for full installation details.

---

## ".NET 10 SDK not found"

**Cause:** The .NET 10 SDK isn't installed, or an older version is active.

**Fix:**

1. Check your version:
   ```bash
   dotnet --version
   ```

2. If missing or below 10.0, install from [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

3. If you have multiple SDKs, check that `global.json` isn't pinning an older version:
   ```bash
   dotnet --info
   ```

---

## Empty graph / no nodes found

**Cause:** No supported files in the target directory, or pointing at the wrong path.

**Fix:**

1. Check what files graphify sees:
   ```bash
   graphify run ./your-project --verbose
   ```
   The verbose output shows detected files and their types.

2. Make sure you're pointing at a directory with source code, not a build output folder (`bin/`, `obj/`, `dist/`).

3. Verify the directory contains supported file types. graphify supports: `.cs`, `.py`, `.ts`, `.js`, `.go`, `.rs`, `.java`, `.c`, `.cpp`, `.rb`, `.kt`, `.scala`, `.php`, `.swift`, `.r`, `.lua`, and more. See the [README](../README.md#supported-languages) for the full list.

4. Check if files are git-ignored. By default, graphify respects `.gitignore`. Untracked files in a git repo may be skipped.

---

## AI provider errors

### Ollama: "Connection refused"

**Cause:** Ollama server isn't running.

**Fix:**
```bash
# Start the server
ollama serve

# Verify it's running
curl http://localhost:11434/api/tags
```

On Windows, check that Ollama is running in the system tray. See [Ollama Setup](setup-ollama.md).

### Ollama: "Model not found"

**Cause:** The model hasn't been pulled yet.

**Fix:**
```bash
ollama pull llama3.2
ollama list  # verify it's there
```

### Azure OpenAI: "401 Unauthorized" or "403 Forbidden"

**Cause:** Invalid or expired API key, or wrong endpoint/deployment.

**Fix:**
1. Verify your endpoint, API key, and deployment name:
   ```bash
   graphify config show
   ```
2. Re-run the config wizard to update credentials:
   ```bash
   graphify config set
   ```
3. Check that your Azure OpenAI resource is active and the deployment exists.

See [Azure OpenAI Setup](setup-azure-openai.md).

### "No AI provider configured"

**Cause:** You're running without any provider, or the config is missing.

**Fix:** This is fine! AST-only mode works without any AI provider. If you want AI enrichment, run:
```bash
graphify config
```

---

## Large projects are slow

**Cause:** graphify processes every supported file in the directory tree. Large monorepos with thousands of files take longer.

**Fix:**

1. **Use AST-only mode** — skip the AI provider (which is the slowest stage):
   ```bash
   graphify run . --provider none
   ```
   Or simply don't configure a provider. AST extraction is fast.

2. **Target a subdirectory** instead of the whole repo:
   ```bash
   graphify run ./src/MyModule
   ```

3. **Use `--verbose`** to see where time is spent — the bottleneck is usually AI semantic extraction, not AST parsing.

---

## Watch mode not detecting changes

**Cause:** Filesystem watcher limitations vary by OS.

**Fix:**

1. Verify watch mode is running:
   ```bash
   graphify watch . --verbose
   ```

2. **Linux:** The default inotify watch limit may be too low. Increase it:
   ```bash
   echo fs.inotify.max_user_watches=524288 | sudo tee -a /etc/sysctl.conf
   sudo sysctl -p
   ```

3. **Network drives / Docker volumes:** Filesystem watchers don't work across network boundaries. Run graphify on the local filesystem.

4. **Debounce:** Changes are batched with a 500ms debounce window. Rapid saves may appear delayed.

See [Watch Mode](watch-mode.md) for details.

---

## graph.html shows a blank page

**Cause:** Some browsers block local file access due to CORS / `file://` restrictions.

**Fix:**

1. **Try a different browser.** Chrome is stricter than Firefox about `file://` access.

2. **Use a local HTTP server:**
   ```bash
   # Python
   python -m http.server 8000 -d graphify-out

   # .NET
   dotnet serve -d graphify-out -p 8000

   # Node.js
   npx serve graphify-out
   ```
   Then open `http://localhost:8000/graph.html`.

3. **Check the file size.** Very large graphs (5000+ nodes) may take several seconds to render. Wait for the page to load fully.

---

## "How do I use this without AI?"

AST-only mode is the default when no AI provider is configured. It works with zero setup:

```bash
graphify run .
```

AST extraction parses source code structure — classes, functions, imports, and their relationships. It produces `EXTRACTED` confidence nodes. This is fast, deterministic, and requires no API keys or external services.

AI providers add `INFERRED` nodes — semantic relationships that can't be detected from syntax alone. But AST-only mode is fully functional and produces useful graphs for any codebase.

To explicitly disable AI even if configured:

```bash
graphify run . --provider none
```

---

## Version conflicts or upgrade issues

**Fix:** Force a clean reinstall:

```bash
dotnet tool uninstall -g graphify-dotnet
dotnet tool install -g graphify-dotnet
```

Check installed version:
```bash
dotnet tool list -g | grep graphify
```

---

## Still stuck?

1. Run with `--verbose` — it shows every pipeline stage and what's happening
2. Check the [CLI Reference](cli-reference.md) for correct command syntax
3. Open an issue on [GitHub](https://github.com/elbruno/graphify-dotnet)
