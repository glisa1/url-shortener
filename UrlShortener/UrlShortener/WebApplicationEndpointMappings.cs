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
        app.MapDeleteEndpoints();
    }

    private static void MapPostEndpoints(this WebApplication app)
    {
        app.MapPost("/shortenurl",
            async 
            (
                ShortenUrlRequest request,
                HttpContext context,
                IPersistanceService redisService,
                IValidator<ShortenUrlRequest> validator,
                Serilog.ILogger logger,
                CancellationToken token
            ) =>
        {
            try
            {
                var validationResult = validator.Validate(request);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var urlHash = HashComputer.GetHashString(request.Url);
                var hashedValue = urlHash[..6];
                var shortAddress = $"http://{context.Request.Host}/{hashedValue}";

                var response = new ShortenedUrl(hashedValue, request.Url, shortAddress);
                await redisService.InsertKeyAsync(response.ShortUrl, response, token);

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unexpected error");
                return Results.Problem();
            }

        })
        .WithName("ShortenUrl")
        .WithOpenApi();
    }

    private static void MapGetEndpoints(this WebApplication app)
    {
        app.MapGet("/shortenedurl",
            async 
            (
                string url,
                IPersistanceService redisService,
                Serilog.ILogger logger,
                CancellationToken token
            ) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return Results.BadRequest("Url was empty.");
                }

                if (!StringExtensions.IsStringAUrl(url))
                {
                    return Results.BadRequest("Input string is not a valid url value.");
                }

                var response = await redisService.GetValueAsync(url, token);

                if (response == null)
                {
                    return Results.NotFound();
                }

                return Results.Redirect(response.LongUrl);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unexpected error");
                return Results.Problem();
            }

        })
        .WithName("ShortenedUrl")
        .WithOpenApi();
    }

    private static void MapDeleteEndpoints(this WebApplication app)
    {
        app.MapDelete("/deleteshortenedurl", 
            async 
            (
                string url,
                IPersistanceService redisService,
                Serilog.ILogger logger,
                CancellationToken token
            ) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return Results.BadRequest("Url was empty.");
                }

                if (!StringExtensions.IsStringAUrl(url))
                {
                    return Results.BadRequest("Input string is not a valid url value.");
                }

                var response = await redisService.GetValueAsync(url, token);

                if (response == null)
                {
                    return Results.NotFound();
                }

                await redisService.DeleteValueAsync(url, token);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unexpected error");
                return Results.Problem();
            }
        });
    }
}
