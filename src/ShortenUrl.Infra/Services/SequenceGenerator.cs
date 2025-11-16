using ShortenUrl.Domain.Services;
using StackExchange.Redis;

namespace ShortenUrl.Infra.Services;

public class SequenceGenerator : ISequenceGenerator
{
    private readonly IDatabase _db;
    private const string CounterKey = "global:sequence";
    
    public SequenceGenerator(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
        InitializeIfNeeded();
    }

    private void InitializeIfNeeded()
    {
        // Caso não exista, inicializa o contador com 15000000
        _db.StringSet(CounterKey, 15000000, when: When.NotExists);
    }
    
    public async Task<long> GetNextIdAsync()
    {
        var value = await _db.StringIncrementAsync(CounterKey);
        return value;
    }
}