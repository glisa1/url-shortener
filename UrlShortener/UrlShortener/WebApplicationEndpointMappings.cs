﻿using UrlShortener.Models;
using UrlShortener.Utils;

namespace UrlShortener;

internal static class WebApplicationEndpointMappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/shortenurl", (ShortenUrlRequest request) =>
        {
            // Should inject the validator
            var validator = new ShortenUrlRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var hashedValue = HashComputer.GetHashString(request.Url);

            return Results.Ok(hashedValue);
            
        })
        .WithName("ShortenUrl")
        .WithOpenApi();
    }
}
