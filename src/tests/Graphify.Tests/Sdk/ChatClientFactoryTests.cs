using Xunit;

namespace Graphify.Tests.Sdk;

/// <summary>
/// Tests for ChatClientFactory, AiProvider enum, and AiProviderOptions.
/// Factory tests are commented out until implementation lands in Graphify.Sdk.
/// Enum and record contract tests use inline definitions to validate the expected API shape.
/// </summary>
public class ChatClientFactoryTests
{
    // ──────────────────────────────────────────────
    // AiProvider enum contract tests
    // Uses inline enum to validate expected shape.
    // Replace with `using Graphify.Sdk;` when real type lands.
    // ──────────────────────────────────────────────

    /// <summary>
    /// Expected shape of the AiProvider enum.
    /// Remove this and use the real type from Graphify.Sdk once implemented.
    /// </summary>
    private enum AiProvider
    {
        AzureOpenAI,
        Ollama
    }

    /// <summary>
    /// Expected shape of the AiProviderOptions record.
    /// Remove this and use the real type from Graphify.Sdk once implemented.
    /// </summary>
    private record AiProviderOptions(
        AiProvider Provider,
        string ApiKey = "",
        string Endpoint = "",
        string ModelId = "",
        string DeploymentName = "");

    [Fact]
    [Trait("Category", "Sdk")]
    public void AiProvider_HasExpectedValues()
    {
        Assert.True(Enum.IsDefined(typeof(AiProvider), AiProvider.AzureOpenAI));
        Assert.True(Enum.IsDefined(typeof(AiProvider), AiProvider.Ollama));
    }

    [Fact]
    [Trait("Category", "Sdk")]
    public void AiProvider_HasExactlyTwoValues()
    {
        var values = Enum.GetValues<AiProvider>();
        Assert.Equal(2, values.Length);
    }

    [Fact]
    [Trait("Category", "Sdk")]
    public void AiProviderOptions_AzureOpenAI_ConstructionWorks()
    {
        var options = new AiProviderOptions(
            Provider: AiProvider.AzureOpenAI,
            ApiKey: "azure-key",
            Endpoint: "https://myinstance.openai.azure.com/",
            DeploymentName: "gpt-4o");

        Assert.Equal(AiProvider.AzureOpenAI, options.Provider);
        Assert.Equal("azure-key", options.ApiKey);
        Assert.Equal("https://myinstance.openai.azure.com/", options.Endpoint);
        Assert.Equal("gpt-4o", options.DeploymentName);
    }

    [Fact]
    [Trait("Category", "Sdk")]
    public void AiProviderOptions_Ollama_ConstructionWorks()
    {
        var options = new AiProviderOptions(
            Provider: AiProvider.Ollama,
            Endpoint: "http://localhost:11434",
            ModelId: "llama3.2");

        Assert.Equal(AiProvider.Ollama, options.Provider);
        Assert.Equal("http://localhost:11434", options.Endpoint);
        Assert.Equal("llama3.2", options.ModelId);
    }

    [Fact]
    [Trait("Category", "Sdk")]
    public void AiProviderOptions_RecordEquality_WorksCorrectly()
    {
        var a = new AiProviderOptions(Provider: AiProvider.Ollama, ModelId: "llama3.2");
        var b = new AiProviderOptions(Provider: AiProvider.Ollama, ModelId: "llama3.2");

        Assert.Equal(a, b);
    }

    [Fact]
    [Trait("Category", "Sdk")]
    public void AiProviderOptions_DefaultValues_AreEmpty()
    {
        var options = new AiProviderOptions(Provider: AiProvider.AzureOpenAI);

        Assert.Equal("", options.ApiKey);
        Assert.Equal("", options.Endpoint);
        Assert.Equal("", options.ModelId);
        Assert.Equal("", options.DeploymentName);
    }

    // ──────────────────────────────────────────────
    // ChatClientFactory.Create tests
    // TODO: Uncomment when ChatClientFactory lands in Graphify.Sdk
    // ──────────────────────────────────────────────

    // [Fact]
    // [Trait("Category", "Sdk")]
    // public void Create_UnknownProvider_ThrowsArgumentException()
    // {
    //     var options = new Graphify.Sdk.AiProviderOptions(
    //         Provider: (Graphify.Sdk.AiProvider)999,
    //         ApiKey: "key");
    //
    //     Assert.Throws<ArgumentException>(() =>
    //         ChatClientFactory.Create(options));
    // }

    // [Fact]
    // [Trait("Category", "Sdk")]
    // public void Create_NullOptions_ThrowsArgumentNullException()
    // {
    //     Assert.Throws<ArgumentNullException>(() =>
    //         ChatClientFactory.Create(null!));
    // }
}
