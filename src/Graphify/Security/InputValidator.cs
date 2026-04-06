using System.Net;
using System.Text.RegularExpressions;

namespace Graphify.Security;

/// <summary>
/// Implementation of security validation for URLs, paths, labels, and general input.
/// Ported from Python graphify/security.py.
/// </summary>
public partial class InputValidator : ISecurityValidator
{
    private static readonly string[] AllowedSchemes = { "http", "https" };
    private const int DefaultMaxLabelLength = 200;
    
    // Private IP ranges to block
    private static readonly string[] PrivateIpPrefixes =
    {
        "10.", "127.", "192.168.",
        "172.16.", "172.17.", "172.18.", "172.19.",
        "172.20.", "172.21.", "172.22.", "172.23.",
        "172.24.", "172.25.", "172.26.", "172.27.",
        "172.28.", "172.29.", "172.30.", "172.31."
    };

    // Regex patterns
    [GeneratedRegex(@"[\x00-\x1f\x7f]", RegexOptions.Compiled)]
    private static partial Regex ControlCharPattern();
    
    [GeneratedRegex(@"<script[^>]*>.*?</script>|<[^>]+>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex HtmlTagPattern();
    
    [GeneratedRegex(@"[';""\\<>&]", RegexOptions.Compiled)]
    private static partial Regex InjectionPattern();

    public ValidationResult ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return ValidationResult.Failure("URL cannot be null or empty");
        }

        // Parse URL
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return ValidationResult.Failure($"Invalid URL format: {url}");
        }

        // Check scheme
        if (!AllowedSchemes.Contains(uri.Scheme.ToLowerInvariant()))
        {
            return ValidationResult.Failure(
                $"Blocked URL scheme '{uri.Scheme}' - only http and https are allowed");
        }

        // Check for private/internal IPs
        var host = uri.Host.ToLowerInvariant();
        
        // Check for localhost
        if (host == "localhost" || host == "::1")
        {
            return ValidationResult.Failure($"Access to localhost is blocked: {url}");
        }

        // Try to resolve to IP and check if it's private
        if (IPAddress.TryParse(host, out var ipAddress))
        {
            if (IsPrivateIp(ipAddress))
            {
                return ValidationResult.Failure($"Access to private IP address is blocked: {url}");
            }
        }
        else
        {
            // For hostnames, check if they start with private IP patterns
            foreach (var prefix in PrivateIpPrefixes)
            {
                if (host.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return ValidationResult.Failure($"Access to private IP range is blocked: {url}");
                }
            }
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePath(string path, string? baseDirectory = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return ValidationResult.Failure("Path cannot be null or empty");
        }

        // Check for null bytes
        if (path.Contains('\0'))
        {
            return ValidationResult.Failure("Path contains null bytes");
        }

        // Check for path traversal patterns
        if (path.Contains("..", StringComparison.Ordinal))
        {
            return ValidationResult.Failure("Path traversal detected: '..' is not allowed");
        }

        try
        {
            // Normalize the path
            var fullPath = Path.GetFullPath(path);

            // If base directory is specified, ensure path is within it
            if (!string.IsNullOrWhiteSpace(baseDirectory))
            {
                var basePath = Path.GetFullPath(baseDirectory);
                
                // Check if base directory exists
                if (!Directory.Exists(basePath))
                {
                    return ValidationResult.Failure(
                        $"Base directory does not exist: {basePath}");
                }

                // Ensure the resolved path is within the base
                if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                {
                    return ValidationResult.Failure(
                        $"Path escapes the allowed directory {basePath}");
                }
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Invalid path: {ex.Message}");
        }
    }

    public ValidationResult SanitizeLabel(string label, int maxLength = DefaultMaxLabelLength)
    {
        if (string.IsNullOrEmpty(label))
        {
            return ValidationResult.Success(string.Empty);
        }

        // Remove control characters
        var sanitized = ControlCharPattern().Replace(label, string.Empty);

        // Remove HTML and script tags
        sanitized = HtmlTagPattern().Replace(sanitized, string.Empty);

        // Limit length
        if (sanitized.Length > maxLength)
        {
            sanitized = sanitized[..maxLength];
        }

        // HTML escape special characters
        sanitized = System.Net.WebUtility.HtmlEncode(sanitized);

        return ValidationResult.Success(sanitized);
    }

    public ValidationResult ValidateInput(string input, int maxLength = 1000)
    {
        if (input == null)
        {
            return ValidationResult.Failure("Input cannot be null");
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            return ValidationResult.Failure("Input cannot be empty or whitespace");
        }

        // Check length
        if (input.Length > maxLength)
        {
            return ValidationResult.Failure(
                $"Input exceeds maximum length of {maxLength} characters");
        }

        // Check for null bytes
        if (input.Contains('\0'))
        {
            return ValidationResult.Failure("Input contains null bytes");
        }

        // Check for common injection patterns
        var injectionMatches = InjectionPattern().Matches(input);
        if (injectionMatches.Count > input.Length * 0.1) // More than 10% special chars
        {
            return ValidationResult.Failure(
                "Input contains suspicious injection patterns");
        }

        // Check for control characters
        var controlChars = ControlCharPattern().Matches(input);
        if (controlChars.Count > 0)
        {
            return ValidationResult.Failure("Input contains control characters");
        }

        return ValidationResult.Success();
    }

    private static bool IsPrivateIp(IPAddress ipAddress)
    {
        var bytes = ipAddress.GetAddressBytes();

        // IPv4 private ranges
        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            // 10.0.0.0/8
            if (bytes[0] == 10)
                return true;

            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                return true;

            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168)
                return true;

            // 127.0.0.0/8 (loopback)
            if (bytes[0] == 127)
                return true;
        }
        // IPv6 private ranges
        else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            // ::1 (loopback)
            if (IPAddress.IsLoopback(ipAddress))
                return true;

            // fc00::/7 (unique local addresses)
            if (bytes[0] >= 0xfc && bytes[0] <= 0xfd)
                return true;

            // fe80::/10 (link-local)
            if (bytes[0] == 0xfe && (bytes[1] & 0xc0) == 0x80)
                return true;
        }

        return false;
    }
}
