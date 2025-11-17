namespace ShortenUrl.Domain.Services;

public interface IShortenUrlService
{
    Task<string> ShortenAsync(string originalUrl);
    Task<string> GetOriginalUrlAsync(string shortCode);
}