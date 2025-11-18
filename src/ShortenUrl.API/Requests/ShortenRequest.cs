namespace ShortenUrl.API.Requests;

public record ShortenRequest
{
    public string OriginalUrl { get; init; } = string.Empty;
}