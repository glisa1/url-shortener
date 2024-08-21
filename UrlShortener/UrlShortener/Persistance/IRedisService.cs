using UrlShortener.Models;

namespace UrlShortener.Persistance;

public interface IRedisService
{
    public Task InsertKeyAsync(string key, ShortenedUrl value, CancellationToken token = default);
    public Task<ShortenedUrl> GetValueAsync(string key, CancellationToken token = default);
}
