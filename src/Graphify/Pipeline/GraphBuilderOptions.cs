namespace Graphify.Pipeline;

/// <summary>
/// Configuration options for the GraphBuilder pipeline stage.
/// </summary>
public record GraphBuilderOptions
{
    /// <summary>
    /// Strategy for merging duplicate nodes.
    /// </summary>
    public MergeStrategy MergeStrategy { get; init; } = MergeStrategy.HighestConfidence;

    /// <summary>
    /// Whether to create file-level nodes for each source file.
    /// File nodes allow tracking which entities belong to which files.
    /// </summary>
    public bool CreateFileNodes { get; init; } = true;

    /// <summary>
    /// Minimum edge weight threshold. Edges below this weight are discarded.
    /// Default 0.0 means all edges are kept.
    /// </summary>
    public double MinEdgeWeight { get; init; } = 0.0;
}

/// <summary>
/// Strategy for handling duplicate nodes during graph construction.
/// </summary>
public enum MergeStrategy
{
    /// <summary>
    /// Keep the node with the highest confidence level.
    /// Extracted > Inferred > Ambiguous.
    /// </summary>
    HighestConfidence,

    /// <summary>
    /// Keep the most recently extracted node (last extraction wins).
    /// </summary>
    MostRecent,

    /// <summary>
    /// Aggregate metadata from all duplicate nodes.
    /// Confidence is set to the highest, metadata is merged.
    /// </summary>
    Aggregate
}
