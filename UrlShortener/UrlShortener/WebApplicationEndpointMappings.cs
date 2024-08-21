﻿using FluentValidation;
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
            async (
                ShortenUrlRequest request,
                CancellationToken token,
                HttpContext context,
                IRedisService redisService,
                IValidator<ShortenUrlRequest> validator
            ) =>
        {
            // Exception handling
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var urlHash = HashComputer.GetHashString(request.Url);
            var hashedValue = urlHash[..6];
            var shortAddress = $"http://{context.Request.Host}/{hashedValue}";

            var response = new ShortenedUrl(hashedValue, request.Url, shortAddress);
            await redisService.InsertKeyAsync(shortAddress, response, token);

            return Results.Ok(response);

        })
        .WithName("ShortenUrl")
        .WithOpenApi();
    }

    private static void MapGetEndpoints(this WebApplication app)
    {
        app.MapGet("/shortenedurl", async (string url, IRedisService redisService, CancellationToken token) =>
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Results.BadRequest("Url was empty.");
            }

            var response = await redisService.GetValueAsync(url, token);

            return Results.Ok(response);

        })
        .WithName("ShortenedUrl")
        .WithOpenApi();
    }
}
