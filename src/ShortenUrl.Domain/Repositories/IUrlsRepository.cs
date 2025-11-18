using ShortenUrl.Domain.Entities;

namespace ShortenUrl.Domain.Repositories;

public interface IUrlsRepository
{
    Task AddUrlAsync(Url url, CancellationToken ct);
    Task<Url?> GetUrlByShortCodeAsync(string shortCode, CancellationToken ct);
}