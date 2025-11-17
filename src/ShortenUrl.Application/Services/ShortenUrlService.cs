using ShortenUrl.Application.Utils;
using ShortenUrl.Domain.Services;

namespace ShortenUrl.Application.Services;

public class ShortenUrlService(
    ISequenceGenerator sequenceGenerator) : IShortenUrlService
{
    
    public async Task<string> ShortenAsync(string originalUrl)
    {
        var id = await sequenceGenerator.GetNextIdAsync();
        var shortCode = EncodeBase62.Encode(id);
     
        // Configurar banco de dados e salvar o mapeamento entre shortCode e originalUrl
        return shortCode;
    }

    public async Task<string> GetOriginalUrlAsync(string shortCode)
    {
        return string.Empty;
    }
}