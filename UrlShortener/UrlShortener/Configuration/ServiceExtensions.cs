using FluentValidation;
using Serilog;
using UrlShortener.Models;
using UrlShortener.Persistance;

namespace UrlShortener.Configuration;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ShortenUrlRequest>, ShortenUrlRequestValidator>();
        services.AddSingleton<IPersistanceService, RedisService>();
        services.AddSingleton<Serilog.ILogger>(new LoggerConfiguration().WriteTo.Console().CreateLogger());
    }
}
