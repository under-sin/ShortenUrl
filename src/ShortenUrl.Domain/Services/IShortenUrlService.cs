namespace ShortenUrl.Domain.Services;

public interface IShortenUrlService
{
    Task<string> Shorten(string originalUrl);
    Task<string> GetOriginalUrl(string shortCode);
}