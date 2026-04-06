namespace Graphify.Security;

/// <summary>
/// Handles security validation and sensitive data detection.
/// </summary>
public interface ISecurityValidator
{
    /// <summary>
    /// Validates a URL for safe use (scheme check, no private IPs).
    /// </summary>
    ValidationResult ValidateUrl(string url);

    /// <summary>
    /// Validates a file path for safety (no path traversal, within allowed root).
    /// </summary>
    ValidationResult ValidatePath(string path, string? baseDirectory = null);

    /// <summary>
    /// Sanitizes a label for safe display (removes HTML/script, limits length).
    /// </summary>
    ValidationResult SanitizeLabel(string label, int maxLength);

    /// <summary>
    /// Validates general input (checks for injection patterns, null/empty).
    /// </summary>
    ValidationResult ValidateInput(string input, int maxLength);
}
