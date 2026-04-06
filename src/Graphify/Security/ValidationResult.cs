namespace Graphify.Security;

/// <summary>
/// Result of a validation or sanitization operation.
/// </summary>
/// <param name="IsValid">Whether the validation passed.</param>
/// <param name="Errors">List of validation error messages (empty if valid).</param>
/// <param name="SanitizedValue">Sanitized value for sanitization operations (null for validation-only).</param>
public record ValidationResult(
    bool IsValid,
    IReadOnlyList<string> Errors,
    string? SanitizedValue = null)
{
    public static ValidationResult Success(string? sanitizedValue = null) =>
        new(true, Array.Empty<string>(), sanitizedValue);

    public static ValidationResult Failure(params string[] errors) =>
        new(false, errors, null);

    public static ValidationResult Failure(IEnumerable<string> errors) =>
        new(false, errors.ToList(), null);
}
