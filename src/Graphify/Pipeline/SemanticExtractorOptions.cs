namespace Graphify.Pipeline;

/// <summary>
/// Configuration options for SemanticExtractor.
/// </summary>
public class SemanticExtractorOptions
{
    /// <summary>
    /// Optional model ID to use for extraction.
    /// If null, uses the default model from IChatClient.
    /// </summary>
    public string? ModelId { get; set; }

    /// <summary>
    /// Maximum tokens to generate in the response.
    /// Default: 4096
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// Temperature for generation (0.0 = deterministic, 1.0 = creative).
    /// Default: 0.1 (low temperature for structured extraction)
    /// </summary>
    public float Temperature { get; set; } = 0.1f;

    /// <summary>
    /// Whether to extract semantic concepts from code files.
    /// Default: true
    /// </summary>
    public bool ExtractFromCode { get; set; } = true;

    /// <summary>
    /// Whether to extract concepts from documentation files (.md, .txt, .rst).
    /// Default: true
    /// </summary>
    public bool ExtractFromDocs { get; set; } = true;

    /// <summary>
    /// Whether to extract concepts from media files (.pdf, .png, .jpg).
    /// Requires vision-capable model for images.
    /// Default: true
    /// </summary>
    public bool ExtractFromMedia { get; set; } = true;

    /// <summary>
    /// Maximum number of nodes to extract per file.
    /// Helps control token usage and response size.
    /// Default: 15
    /// </summary>
    public int MaxNodesPerFile { get; set; } = 15;

    /// <summary>
    /// Maximum file size in bytes to process.
    /// Files larger than this are skipped.
    /// Default: 1MB
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 1024 * 1024;
}
