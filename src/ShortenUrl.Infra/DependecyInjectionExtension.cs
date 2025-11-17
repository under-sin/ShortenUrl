using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShortenUrl.Domain.Services;
using ShortenUrl.Infra.DataAccess;
using ShortenUrl.Infra.Services;
using StackExchange.Redis;

namespace ShortenUrl.Infra;

public static class DependecyInjectionExtension
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddDependencies(services);
        AddRedis(services, configuration);
        AddContext(services, configuration);
        AddFluentMigrator(services, configuration);
    }
    
    private static void AddContext(
        IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));
        
        services.AddDbContext<ShortenUrlDbContext>(options 
            => options.UseMySql(connectionString, serverVersion));
    }

    private static void AddFluentMigrator(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection");

        services.AddFluentMigratorCore().ConfigureRunner(rb => rb
            .AddMySql5()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(Assembly.Load("ShortenUrl.Infra")).For.All()
        ).AddLogging(x => x.AddFluentMigratorConsole());
    }
    
    private static void AddDependencies(IServiceCollection services)
    {
        services.AddScoped<ISequenceGenerator, SequenceGenerator>();
    }
    
    private static void AddRedis(
        IServiceCollection services, 
        IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConnectionString!);
        });
    }
}