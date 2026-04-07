using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Graphify.Security;

namespace Graphify.Pipeline;

/// <summary>
/// Validates and sanitizes LLM extraction responses before they enter the pipeline.
/// Guards against prompt injection producing poisoned nodes/edges (FINDING-003).
/// </summary>
public static partial class LlmResponseValidator
{
    private const int MaxNodeLabelLength = 200;
    private const int MaxEdgeRelationLength = 100;
    private const int MaxFilePathLength = 500;
    private const int MaxIdLength = 200;
    private const int MaxNodesAllowed = 50;
    private const int MaxEdgesAllowed = 100;

    private static readonly InputValidator Validator = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [GeneratedRegex(@"<script[^>]*>|</script>|javascript:|on\w+\s*=", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ScriptPattern();

    /// <summary>
    /// Validates and sanitizes a raw JSON LLM response.
    /// Returns a sanitized LlmExtractionData or null if the response is invalid.
    /// </summary>
    public static LlmExtractionData? ValidateAndSanitize(string? rawJson, string filePath)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
            return null;

        // Parse JSON
        LlmExtractionData? data;
        try
        {
            var cleanJson = ExtractJsonFromMarkdown(rawJson);
            data = JsonSerializer.Deserialize<LlmExtractionData>(cleanJson, JsonOptions);
        }
        catch
        {
            return null;
        }

        if (data is null)
            return null;

        // Validate and sanitize nodes
        var sanitizedNodes = new List<LlmNodeData>();
        foreach (var node in data.Nodes ?? [])
        {
            var sanitized = SanitizeNode(node);
            if (sanitized is not null)
                sanitizedNodes.Add(sanitized);
        }

        // Enforce max count
        if (sanitizedNodes.Count > MaxNodesAllowed)
            sanitizedNodes = sanitizedNodes.Take(MaxNodesAllowed).ToList();

        // Validate and sanitize edges — only keep edges referencing known nodes
        var nodeIds = new HashSet<string>(sanitizedNodes.Select(n => n.Id!), StringComparer.OrdinalIgnoreCase);
        var sanitizedEdges = new List<LlmEdgeData>();
        foreach (var edge in data.Edges ?? [])
        {
            var sanitized = SanitizeEdge(edge, nodeIds);
            if (sanitized is not null)
                sanitizedEdges.Add(sanitized);
        }

        if (sanitizedEdges.Count > MaxEdgesAllowed)
            sanitizedEdges = sanitizedEdges.Take(MaxEdgesAllowed).ToList();

        return new LlmExtractionData
        {
            Nodes = sanitizedNodes,
            Edges = sanitizedEdges
        };
    }

    private static LlmNodeData? SanitizeNode(LlmNodeData node)
    {
        // Must have id and label
        if (string.IsNullOrWhiteSpace(node.Id) || string.IsNullOrWhiteSpace(node.Label))
            return null;

        // Reject nodes with script/HTML injection in id or label
        if (ContainsSuspiciousContent(node.Id) || ContainsSuspiciousContent(node.Label))
            return null;

        // Sanitize label through InputValidator
        var labelResult = Validator.SanitizeLabel(node.Label, MaxNodeLabelLength);
        var sanitizedLabel = labelResult.IsValid ? (labelResult.SanitizedValue ?? node.Label) : node.Label;

        // Truncate id
        var sanitizedId = Truncate(node.Id, MaxIdLength);

        // Sanitize metadata values
        Dictionary<string, string>? sanitizedMetadata = null;
        if (node.Metadata is not null)
        {
            sanitizedMetadata = new Dictionary<string, string>();
            foreach (var kvp in node.Metadata)
            {
                if (string.IsNullOrEmpty(kvp.Key) || ContainsSuspiciousContent(kvp.Value ?? ""))
                    continue;

                var metaResult = Validator.SanitizeLabel(kvp.Value ?? "", MaxNodeLabelLength);
                sanitizedMetadata[kvp.Key] = metaResult.IsValid ? (metaResult.SanitizedValue ?? kvp.Value ?? "") : "";
            }
        }

        return new LlmNodeData
        {
            Id = sanitizedId,
            Label = sanitizedLabel,
            Type = Truncate(node.Type ?? "Code", MaxEdgeRelationLength),
            Metadata = sanitizedMetadata
        };
    }

    private static LlmEdgeData? SanitizeEdge(LlmEdgeData edge, HashSet<string> validNodeIds)
    {
        if (string.IsNullOrWhiteSpace(edge.Source) || string.IsNullOrWhiteSpace(edge.Target))
            return null;

        // Only allow edges that reference known nodes
        if (!validNodeIds.Contains(edge.Source) || !validNodeIds.Contains(edge.Target))
            return null;

        // Reject edges with suspicious content
        if (ContainsSuspiciousContent(edge.Relation ?? ""))
            return null;

        // Sanitize relation through InputValidator
        var relationResult = Validator.SanitizeLabel(edge.Relation ?? "related_to", MaxEdgeRelationLength);
        var sanitizedRelation = relationResult.IsValid
            ? (relationResult.SanitizedValue ?? edge.Relation ?? "related_to")
            : "related_to";

        // Clamp weight to valid range
        var weight = edge.Weight ?? 1.0;
        weight = Math.Clamp(weight, 0.0, 1.0);

        return new LlmEdgeData
        {
            Source = edge.Source,
            Target = edge.Target,
            Relation = sanitizedRelation,
            Confidence = edge.Confidence,
            Weight = weight
        };
    }

    private static bool ContainsSuspiciousContent(string value)
    {
        return ScriptPattern().IsMatch(value);
    }

    private static string Truncate(string value, int maxLength)
    {
        if (value.Length <= maxLength)
            return value;
        return value[..(maxLength - 3)] + "...";
    }

    private static string ExtractJsonFromMarkdown(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.StartsWith("```json"))
            trimmed = trimmed[7..];
        else if (trimmed.StartsWith("```"))
            trimmed = trimmed[3..];

        if (trimmed.EndsWith("```"))
            trimmed = trimmed[..^3];

        return trimmed.Trim();
    }

    // DTOs for LLM response deserialization (shared with SemanticExtractor)
    public class LlmExtractionData
    {
        public List<LlmNodeData>? Nodes { get; set; }
        public List<LlmEdgeData>? Edges { get; set; }
    }

    public class LlmNodeData
    {
        public string? Id { get; set; }
        public string? Label { get; set; }
        public string? Type { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class LlmEdgeData
    {
        public string? Source { get; set; }
        public string? Target { get; set; }
        public string? Relation { get; set; }
        public string? Confidence { get; set; }
        public double? Weight { get; set; }
    }
}
