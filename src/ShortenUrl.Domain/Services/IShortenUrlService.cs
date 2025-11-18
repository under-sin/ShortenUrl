namespace ShortenUrl.Domain.Services;

public interface IShortenUrlService
{
    Task<string> ShortenAsync(string originalUrl, CancellationToken ct);
    Task<string?> GetOriginalUrlAsync(string shortCode, CancellationToken ct);
}