namespace UrlShortener.Utils
{
    public static class StringExtensions
    {
        public static bool IsLinkAUrl(this string inputString)
        {
            return Uri.TryCreate(inputString, UriKind.Absolute, out var outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
