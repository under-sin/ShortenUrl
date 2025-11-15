using ShortenUrl.Domain.Services;

namespace ShortenUrl.Application.Services;

public class ShortenUrlService : IShortenUrlService
{
    public string Shorten(string originalUrl)
    {
        // Placeholder implementation
        return "abc123";
    }

    public string GetOriginalUrl(string shortCode)
    {
        throw new NotImplementedException();
    }
}