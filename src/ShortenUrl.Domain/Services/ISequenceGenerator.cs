namespace ShortenUrl.Domain.Services;

public interface ISequenceGenerator
{
    Task<long> GetNextIdAsync();
}