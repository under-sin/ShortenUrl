using System.ComponentModel.DataAnnotations;

namespace ShortenUrl.API.Requests;

public record ShortenRequest
{
    [Required(ErrorMessage = "URL is required")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string OriginalUrl { get; init; } = string.Empty;
}