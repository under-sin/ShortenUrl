using ShortenUrl.Application.Utils;
using ShortenUrl.Domain.Entities;
using ShortenUrl.Domain.Repositories;
using ShortenUrl.Domain.Services;

namespace ShortenUrl.Application.Services;

public class ShortenUrlService(
    ISequenceGenerator sequenceGenerator,
    IUrlsRepository repository) : IShortenUrlService
{

    public async Task<string> ShortenAsync(string originalUrl, CancellationToken ct)
    {
        var id = await sequenceGenerator.GetNextIdAsync();
        var shortCode = EncodeBase62.Encode(id);

        var urlEntity = new Url(shortCode, originalUrl);

        try
        {
            await repository.AddUrlAsync(urlEntity, ct);
        }
        catch (Exception e)
        {
            // configurar o global handler exception
            throw new Exception(e.Message);
        }

        return shortCode;
    }

    public async Task<string?> GetOriginalUrlAsync(string shortCode, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
            return null;

        var urlEntity = await repository.GetUrlByShortCodeAsync(shortCode, ct);
        
        return urlEntity?.OriginalUrl;
    }
}