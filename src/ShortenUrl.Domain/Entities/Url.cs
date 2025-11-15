namespace ShortenUrl.Domain.Entities;

public record Url(
    string ShortCode,
    string OriginalUrl,
    DateTime CreatedAt);