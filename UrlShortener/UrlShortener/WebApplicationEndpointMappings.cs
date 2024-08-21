using UrlShortener.Models;
using UrlShortener.Utils;

namespace UrlShortener;

internal static class WebApplicationEndpointMappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/shortenurl", (ShortenUrlRequest request, HttpContext context) =>
        {
            // Should inject the validator
            var validator = new ShortenUrlRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var urlHash = HashComputer.GetHashString(request.Url);
            var hashedValue = urlHash.Substring(0, 6);
            var shortAddress = $"http://{context.Request.Host}/{hashedValue}";

            var response = new ShortenUrlResponse(hashedValue, request.Url, shortAddress);

            return Results.Ok(response);
            
        })
        .WithName("ShortenUrl")
        .WithOpenApi();
    }
}
