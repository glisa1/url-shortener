using FluentValidation;
using UrlShortener.Utils;

namespace UrlShortener.Models;

internal sealed class ShortenUrlRequest
{
    public ShortenUrlRequest(string url)
    {
        Url = url;
    }

    public string Url { get; init; }
}

internal sealed class ShortenUrlRequestValidator : AbstractValidator<ShortenUrlRequest>
{
    public ShortenUrlRequestValidator()
    {
        RuleFor(request => request.Url)
            .NotEmpty()
            .Must(StringExtensions.IsLinkAUrl)
            .WithMessage("Input string is not a valid url value.");
    }
}

public sealed record ShortenedUrl(string Key, string LongUrl, string ShortUrl);