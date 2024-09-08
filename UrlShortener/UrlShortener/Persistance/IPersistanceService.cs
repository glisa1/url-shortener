using UrlShortener.Models;

namespace UrlShortener.Persistance;

public interface IPersistanceService
{
    public Task InsertKeyAsync(string key, ShortenedUrl value, CancellationToken token = default);
    public Task<ShortenedUrl> GetValueAsync(string key, CancellationToken token = default);

    public Task DeleteValueAsync(string key, CancellationToken token = default);
}
