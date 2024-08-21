using FluentValidation;

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
            .Must(LinkMustBeAUri)
            .WithMessage("Input string is not a valid url value.");
    }

    private static bool LinkMustBeAUri(string link)
    {
        return Uri.TryCreate(link, UriKind.Absolute, out var outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
    }
}