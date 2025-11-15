namespace ShortenUrl.Domain.Services;

public interface IShortenUrlService
{
    public string Shorten(string originalUrl);
    public string GetOriginalUrl(string shortCode);
}