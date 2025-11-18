using Microsoft.EntityFrameworkCore;
using ShortenUrl.Domain.Entities;
using ShortenUrl.Domain.Repositories;

namespace ShortenUrl.Infra.DataAccess.Repositories;

public class UrlsRepository(
    ShortenUrlDbContext context) : IUrlsRepository
{
    
    public async Task AddUrlAsync(Url url, CancellationToken ct = default)
    {
        await context.Urls.AddAsync(url, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Url?> GetUrlByShortCodeAsync(string shortCode, CancellationToken ct = default)
    {
        var url = await context.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode, ct);
        return url;
    }
}