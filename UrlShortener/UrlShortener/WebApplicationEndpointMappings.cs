using FluentValidation;
using UrlShortener.Utils;
using UrlShortener.Models;
using UrlShortener.Persistance;

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
        app.MapPost("/shortenurl", 
            (
                ShortenUrlRequest request,
                HttpContext context,
                IRedisService redisService,
                IValidator<ShortenUrlRequest> validator
            ) =>
        {
            // Should inject the validator
            // Exceptions
            // Async
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

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
        app.MapGet("/shortenedurl", (string url, IRedisService redisService) =>
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Results.BadRequest("Url was empty.");
            }

            var response = redisService.GetValue(url);

            return Results.Ok(response);

        })
        .WithName("ShortenedUrl")
        .WithOpenApi();
    }
}
