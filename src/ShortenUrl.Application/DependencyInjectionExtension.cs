using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShortenUrl.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
    }
}