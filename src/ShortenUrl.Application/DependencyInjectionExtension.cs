using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShortenUrl.Application.Services;
using ShortenUrl.Application.Utils;
using ShortenUrl.Domain.Services;

namespace ShortenUrl.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        EncodeBase62.Initialize(configuration);

        AddServices(services);
    }
    
    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IShortenUrlService, ShortenUrlService>();
    }
}