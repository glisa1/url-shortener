using FluentValidation;
using UrlShortener.Models;
using UrlShortener.Persistance;

namespace UrlShortener.Configuration;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ShortenUrlRequest>, ShortenUrlRequestValidator>();
        services.AddSingleton<IRedisService, RedisService>();
    }
}
