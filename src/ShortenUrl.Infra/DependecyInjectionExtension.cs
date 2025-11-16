using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShortenUrl.Domain.Services;
using ShortenUrl.Infra.Services;
using StackExchange.Redis;

namespace ShortenUrl.Infra;

public static class DependecyInjectionExtension
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddRedis(services, configuration);
        AddDependencies(services);
    }
    
    private static void AddDependencies(IServiceCollection services)
    {
        services.AddScoped<ISequenceGenerator, SequenceGenerator>();
    }
    
    private static void AddRedis(IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConnectionString!);
        });
    }
}