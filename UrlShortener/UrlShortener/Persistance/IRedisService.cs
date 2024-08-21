using UrlShortener.Models;

namespace UrlShortener.Persistance;

public interface IRedisService
{
    public void InsertKey(string key, ShortenedUrl value);
    public ShortenedUrl GetValue(string key);
}
