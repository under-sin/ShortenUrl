namespace ShortenUrl.Domain.Entities;

public record Url
{
    public string ShortCode { get; init; } = string.Empty;
    public string OriginalUrl { get; init; } = string.Empty; 
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
};