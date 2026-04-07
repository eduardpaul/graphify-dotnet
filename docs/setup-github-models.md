# Using graphify-dotnet with GitHub Models

Leverage GitHub's hosted models for free or at low cost. No Azure subscription needed—just authenticate with your GitHub account.

## Quick Start

1. Create a GitHub personal access token (PAT)
2. Set `GITHUB_TOKEN` environment variable
3. Configure graphify-dotnet with `GitHubModelsClientFactory` or unified `ChatClientFactory`
4. Use GPT-4o, Claude 3.5 Sonnet, and more for free on eligible accounts
5. Start analyzing!

## What is GitHub Models?

GitHub Models is a free inference service providing access to cutting-edge AI models through Microsoft's Azure infrastructure. Perfect for:

- **Learning & experimentation** with state-of-the-art models
- **Prototyping** before committing to production APIs
- **Cost-effective** code analysis (free tier available)
- **No extra setup** if you already have a GitHub account

### Available Models

| Model | Creator | Type | Best For |
|-------|---------|------|----------|
| **gpt-4o** | OpenAI | Multimodal | General coding, complex tasks |
| **gpt-4o-mini** | OpenAI | Multimodal | Fast, lightweight analysis |
| **claude-3.5-sonnet** | Anthropic | Text | Code analysis, reasoning |
| **llama-3.1-405b** | Meta | Text | Large context windows |
| **phi-4** | Microsoft | Text | Lightweight, efficient |

Check the [GitHub Models documentation](https://github.com/marketplace/models) for current availability.

## Prerequisites

- **GitHub Account**: [Sign up free](https://github.com/signup)
- **Personal Access Token**: Generate from GitHub settings
- **API Access**: Request access if needed (usually instant for GitHub Users)

## Step 1: Create a GitHub Personal Access Token (PAT)

### Via GitHub Web UI

1. Go to [GitHub Settings → Developer settings → Personal access tokens](https://github.com/settings/tokens)
2. Click **Generate new token** → **Generate new token (classic)**
3. Fill in the form:
   - **Token name**: `graphify-dotnet` or similar
   - **Expiration**: 30 days, 90 days, or no expiration
   - **Scopes**: 
     - ✅ `read:user` (required)
     - ✅ `repo` (if analyzing private repos)
     - ❌ Do NOT check `write:*` or `delete:*`
4. Click **Generate token**
5. **Copy immediately** (you won't see it again!)

### Via GitHub CLI

```bash
# Requires GitHub CLI installed (https://cli.github.com)
gh auth token --secure
```

### Security Best Practices

- ⚠️ **Never commit your PAT** to source code
- ⚠️ **Rotate regularly** (monthly recommended)
- ⚠️ **Use short expiration** (30 days) for development
- ✅ **Store in environment variables** or secrets manager

```bash
# Store securely
export GITHUB_TOKEN="github_pat_xxxxxxxxxxxxxxxxxxxx"

# Verify it works
curl -H "Authorization: token $GITHUB_TOKEN" \
  https://api.github.com/user
```

## Step 2: Configure graphify-dotnet

### Option A: Direct Factory (Fine-grained Control)

```csharp
using Graphify.Sdk;
using Microsoft.Extensions.AI;

// Create GitHub Models-specific options
var options = new CopilotExtractorOptions
{
    ApiKey = "github_pat_xxxxxxxxxxxxxxxxxxxx",
    ModelId = "gpt-4o",
    Endpoint = "https://models.inference.ai.azure.com",
    MaxTokens = 4096
};

IChatClient client = GitHubModelsClientFactory.Create(options);

// Use the client for analysis
var response = await client.CompleteAsync("Analyze this C# code...");
Console.WriteLine(response.Message);
```

### Option B: Unified Factory (Recommended)

```csharp
using Graphify.Sdk;
using Microsoft.Extensions.AI;

// Use the unified ChatClientFactory for easy provider switching
var aiOptions = new AiProviderOptions(
    Provider: AiProvider.GitHubModels,
    ApiKey: Environment.GetEnvironmentVariable("GITHUB_TOKEN"),
    ModelId: "gpt-4o",
    Endpoint: "https://models.inference.ai.azure.com"
);

IChatClient client = ChatClientFactory.Create(aiOptions);

// Use the client
var response = await client.CompleteAsync("Explain this code...");
Console.WriteLine(response.Message);
```

### Option C: From Environment Variables

```csharp
using Graphify.Sdk;

// Read from environment—perfect for production and CI/CD
var options = new AiProviderOptions(
    Provider: AiProvider.GitHubModels,
    ApiKey: Environment.GetEnvironmentVariable("GITHUB_TOKEN"),
    ModelId: Environment.GetEnvironmentVariable("GITHUB_MODELS_MODEL") ?? "gpt-4o",
    Endpoint: Environment.GetEnvironmentVariable("GITHUB_MODELS_ENDPOINT") 
        ?? "https://models.inference.ai.azure.com"
);

IChatClient client = ChatClientFactory.Create(options);
```

## Full Working Example

```csharp
using System;
using Graphify.Sdk;
using Microsoft.Extensions.AI;

public class GitHubModelsAnalyzer
{
    public static async Task Main(string[] args)
    {
        // 1. Get token from environment
        var gitHubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(gitHubToken))
        {
            Console.WriteLine("Error: GITHUB_TOKEN environment variable not set");
            return;
        }

        // 2. Create options
        var options = new AiProviderOptions(
            Provider: AiProvider.GitHubModels,
            ApiKey: gitHubToken,
            ModelId: "gpt-4o",
            Endpoint: "https://models.inference.ai.azure.com"
        );

        // 3. Create the chat client
        IChatClient client = ChatClientFactory.Create(options);

        // 4. Analyze code
        string codeSnippet = @"
public class DataProcessor {
    public List<T> Filter<T>(List<T> items, Func<T, bool> predicate) 
        => items.Where(predicate).ToList();
    
    public string[] ExtractNames(List<Person> people) 
        => people.Select(p => p.Name).ToArray();
}";

        string prompt = $"Analyze this C# code for best practices:\n\n{codeSnippet}";
        
        Console.WriteLine("Analyzing with GitHub Models (gpt-4o)...");
        var response = await client.CompleteAsync(prompt);
        Console.WriteLine("\nAnalysis:");
        Console.WriteLine(response.Message);
    }
}
```

## Environment Variables

| Variable | Description | Default / Required |
|----------|-------------|---------------------|
| `GITHUB_TOKEN` | GitHub Personal Access Token | **Required** |
| `GITHUB_MODELS_ENDPOINT` | GitHub Models API endpoint | `https://models.inference.ai.azure.com` |
| `GITHUB_MODELS_MODEL` | Model to use | `gpt-4o` |

### Setup Example

**Linux/macOS**:
```bash
export GITHUB_TOKEN="github_pat_xxxxxxxxxxxxxxxxxxxx"
export GITHUB_MODELS_MODEL="gpt-4o"

# Verify
echo $GITHUB_TOKEN
```

**Windows (PowerShell)**:
```powershell
$env:GITHUB_TOKEN = "github_pat_xxxxxxxxxxxxxxxxxxxx"
$env:GITHUB_MODELS_MODEL = "gpt-4o"

# Verify
Write-Host $env:GITHUB_TOKEN
```

**Windows (.NET User Secrets)**:
```bash
dotnet user-secrets set "GITHUB_TOKEN" "github_pat_xxxxxxxxxxxxxxxxxxxx"
```

## Pricing & Rate Limits

### Free Tier

- ✅ **Free inference** for eligible GitHub users
- ✅ **Generous rate limits** (hundreds of requests daily)
- ⚠️ Subject to availability and fair use

### Standard / Enterprise

- 💰 Pay-as-you-go pricing
- 📈 Higher rate limits
- 🏢 Priority support

### Rate Limiting Best Practices

```csharp
// Add exponential backoff for rate-limited responses
int maxRetries = 3;
for (int attempt = 0; attempt < maxRetries; attempt++)
{
    try
    {
        return await client.CompleteAsync(prompt);
    }
    catch (Exception ex) when (ex.Message.Contains("429") && attempt < maxRetries - 1)
    {
        // Wait 2^attempt seconds before retrying
        var delaySeconds = Math.Pow(2, attempt);
        Console.WriteLine($"Rate limited. Retrying in {delaySeconds}s...");
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
    }
}
```

## Model Selection Guide

### For Code Analysis

- **gpt-4o** (best): Most capable, handles complex code structures
- **gpt-4o-mini**: Faster, cheaper, still excellent for analysis
- **claude-3.5-sonnet**: Great for detailed explanations and refactoring suggestions

### For Speed

- **gpt-4o-mini**: Fast inference, minimal latency
- **phi-4**: Lightweight but less capable

### For Large Context

- **llama-3.1-405b**: Large context window (131K tokens), great for full-file analysis
- **gpt-4o**: Good balance of capability and context (128K tokens)

## Code Samples

### Switching Models at Runtime

```csharp
// Easy switching between models
var models = new[] { "gpt-4o", "gpt-4o-mini", "claude-3.5-sonnet" };

foreach (var model in models)
{
    var options = new AiProviderOptions(
        Provider: AiProvider.GitHubModels,
        ApiKey: Environment.GetEnvironmentVariable("GITHUB_TOKEN"),
        ModelId: model
    );
    
    var client = ChatClientFactory.Create(options);
    var response = await client.CompleteAsync("Explain OOP principles");
    Console.WriteLine($"{model}: {response.Message}");
}
```

### Batch Processing with Rate Limit Handling

```csharp
var client = ChatClientFactory.Create(new AiProviderOptions(
    Provider: AiProvider.GitHubModels,
    ApiKey: Environment.GetEnvironmentVariable("GITHUB_TOKEN"),
    ModelId: "gpt-4o"
));

var codeFiles = Directory.GetFiles("src", "*.cs", SearchOption.AllDirectories);
var results = new ConcurrentBag<(string File, string Analysis)>();

var semaphore = new SemaphoreSlim(maxConcurrentRequests: 2);

Parallel.ForEach(codeFiles, new ParallelOptions { MaxDegreeOfParallelism = 2 },
    async (file) =>
    {
        await semaphore.WaitAsync();
        try
        {
            var code = File.ReadAllText(file);
            var response = await client.CompleteAsync($"Analyze: {code}");
            results.Add((file, response.Message));
        }
        finally
        {
            semaphore.Release();
        }
    });

foreach (var (file, analysis) in results)
{
    Console.WriteLine($"{file}: {analysis}");
}
```

## Troubleshooting

### ❌ 401 Unauthorized

**Cause**: Invalid or missing GitHub token

**Solution**:
```bash
# Verify token is set and valid
echo $GITHUB_TOKEN

# Test token with GitHub API
curl -H "Authorization: token $GITHUB_TOKEN" \
  https://api.github.com/user

# If invalid, generate a new PAT from https://github.com/settings/tokens
```

### ❌ 403 Forbidden

**Cause**: GitHub account not eligible for GitHub Models, or token scopes insufficient

**Solution**:
- Check you're [eligible for GitHub Models](https://github.com/marketplace/models)
- Regenerate token with `read:user` scope
- Contact GitHub Support if needed

### ❌ 429 Too Many Requests

**Cause**: Rate limit exceeded

**Solution**:
```csharp
// Add backoff logic (see above)
// Or reduce parallel requests
var semaphore = new SemaphoreSlim(maxConcurrentRequests: 1);
```

### ❌ Unknown Model

**Cause**: Model doesn't exist or isn't available yet

**Solution**:
- Check [GitHub Models marketplace](https://github.com/marketplace/models) for available models
- Use `gpt-4o` or `gpt-4o-mini` (always available)

### ❌ Connection Timeout

**Cause**: Network issues or endpoint not reachable

**Solution**:
```csharp
// Verify endpoint connectivity
using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
try
{
    var response = await http.GetAsync("https://models.inference.ai.azure.com");
    Console.WriteLine($"Endpoint reachable: {response.StatusCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"Endpoint error: {ex.Message}");
}
```

## Security Notes

1. **Never hardcode PATs**:
   ```csharp
   // ❌ BAD - never do this!
   var options = new AiProviderOptions(
       Provider: AiProvider.GitHubModels,
       ApiKey: "github_pat_xxxxxxxxxxxxxxxxxxxx"  // EXPOSED!
   );

   // ✅ GOOD - use environment variables
   var options = new AiProviderOptions(
       Provider: AiProvider.GitHubModels,
       ApiKey: Environment.GetEnvironmentVariable("GITHUB_TOKEN")
   );
   ```

2. **Rotate tokens regularly** to minimize exposure if leaked

3. **Use minimal scopes** - only `read:user` is needed for GitHub Models

4. **In CI/CD**: Use GitHub secrets or Azure Key Vault, not plain environment variables

## See Also

- [Using graphify-dotnet with Azure OpenAI](./setup-azure-openai.md)
- [Using graphify-dotnet with Ollama (Local Models)](./setup-ollama.md)
- [GitHub Models Documentation](https://github.com/marketplace/models)
- [GitHub Personal Access Tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens)
- [API Reference: GitHubModelsClientFactory](../src/Graphify.Sdk/GitHubModelsClientFactory.cs)

## Next Steps

1. Create a GitHub PAT from https://github.com/settings/tokens
2. Set the `GITHUB_TOKEN` environment variable
3. Run the example code above
4. Explore the [README](../README.md) for full SDK capabilities
5. Build and share your code analysis tools!

---

**Need help?** Open an issue on [GitHub](https://github.com/BrunoCapuano/graphify-dotnet) or check the [documentation](../README.md).
