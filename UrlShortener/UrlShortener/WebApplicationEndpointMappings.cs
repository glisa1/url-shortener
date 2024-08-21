using UrlShortener.Models;
using UrlShortener.Persistence;
using UrlShortener.Utils;

namespace UrlShortener;

internal static class WebApplicationEndpointMappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGetEndpoints();
        app.MapPostEndpoints();
    }

    private static void MapPostEndpoints(this WebApplication app)
    {
        app.MapPost("/shortenurl", (ShortenUrlRequest request, HttpContext context) =>
        {
            // Should inject the validator
            // Exceptions
            // Async
            var validator = new ShortenUrlRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var redisService = new RedisService();

            var urlHash = HashComputer.GetHashString(request.Url);
            var hashedValue = urlHash.Substring(0, 6);
            var shortAddress = $"http://{context.Request.Host}/{hashedValue}";

            var response = new ShortenedUrl(hashedValue, request.Url, shortAddress);
            redisService.InsertKey(shortAddress, response);

            return Results.Ok(response);

        })
        .WithName("ShortenUrl")
        .WithOpenApi();
    }

    private static void MapGetEndpoints(this WebApplication app)
    {
        app.MapGet("/shortenedurl", (string url) =>
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Results.BadRequest("Url was empty.");
            }

            var redisService = new RedisService();

            var response = redisService.GetValue(url);

            return Results.Ok(response);

        })
        .WithName("ShortenedUrl")
        .WithOpenApi();
    }
}
