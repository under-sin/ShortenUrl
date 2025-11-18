using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShortenUrl.Domain.Entities;

[Table("Urls")]
public record Url
{
    [Key]
    [MaxLength(7)]
    public string ShortCode { get; init; } = string.Empty;
    public string OriginalUrl { get; init; } = string.Empty; 
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public Url(string shortCode, string originalUrl)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
            throw new ArgumentException("ShortCode cannot be null or empty.", nameof(shortCode));
        
        if (string.IsNullOrWhiteSpace(originalUrl))
            throw new ArgumentException("OriginalUrl cannot be null or empty.", nameof(originalUrl));
        
        ShortCode = shortCode;
        OriginalUrl = originalUrl;
    }
};