using ShortenUrl.Application.Utils;
using ShortenUrl.Domain.Entities;
using ShortenUrl.Domain.Exceptions;
using ShortenUrl.Domain.Repositories;
using ShortenUrl.Domain.Services;

namespace ShortenUrl.Application.Services;

public class ShortenUrlService(
    ISequenceGenerator sequenceGenerator,
    IUrlsRepository repository) : IShortenUrlService
{

    public async Task<string> ShortenAsync(string originalUrl, CancellationToken ct)
    {
        UrlValidator.Validate(originalUrl);
        
        var id = await sequenceGenerator.GetNextIdAsync();
        var shortCode = EncodeBase62.Encode(id);

        var urlEntity = new Url(shortCode, originalUrl);

        await repository.AddUrlAsync(urlEntity, ct);

        return shortCode;
    }

    public async Task<string?> GetOriginalUrlAsync(string shortCode, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
            return null;

        var urlEntity = await repository.GetUrlByShortCodeAsync(shortCode, ct);

        if (urlEntity?.OriginalUrl != null)
            UrlValidator.Validate(urlEntity.OriginalUrl);
        
        return urlEntity?.OriginalUrl;
    }
}