using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ShortenUrl.Domain.Services;
using ShortenUrl.Infra.DataAccess;
using StackExchange.Redis;

namespace ShortenUrl.Tests.Integration;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private SqliteConnection _connection = null!;
    private long _currentSequence = 15000000;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            // Adiciona configurações necessárias para os testes
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Base62:CharacterSet"] = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove o DbContext existente (MySQL)
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ShortenUrlDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Remove o Redis
            var redisDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConnectionMultiplexer));
            if (redisDescriptor != null)
            {
                services.Remove(redisDescriptor);
            }

            // Remove o SequenceGenerator existente
            var sequenceGeneratorDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ISequenceGenerator));
            if (sequenceGeneratorDescriptor != null)
            {
                services.Remove(sequenceGeneratorDescriptor);
            }

            // Adiciona SQLite in-memory
            services.AddDbContext<ShortenUrlDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Mock do SequenceGenerator (substitui o Redis)
            services.AddScoped<ISequenceGenerator>(_ => 
            {
                var mock = new Mock<ISequenceGenerator>();
                mock.Setup(x => x.GetNextIdAsync())
                    .ReturnsAsync(() => Interlocked.Increment(ref _currentSequence));
                return mock.Object;
            });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        // Cria conexão SQLite in-memory
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        // Cria o schema do banco
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShortenUrlDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await base.DisposeAsync();
    }
}

