using ShortenUrl.Domain.Exceptions;

namespace ShortenUrl.Application.Utils;

public static class UrlValidator
{
    private static readonly string[] AllowedSchemes = ["http", "https"];
    private static readonly string[] BlockedSchemes = ["javascript", "data", "file", "vbscript"];

    public static void Validate(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidUrlException("URL cannot be null or empty.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new InvalidUrlException("Invalid URL format.");

        var scheme = uri.Scheme.ToLowerInvariant();

        if (BlockedSchemes.Contains(scheme))
            throw new InvalidUrlException($"URL scheme '{scheme}' is not allowed for security reasons.");

        if (!AllowedSchemes.Contains(scheme))
            throw new InvalidUrlException($"Only HTTP and HTTPS URLs are allowed.");
    }
}
