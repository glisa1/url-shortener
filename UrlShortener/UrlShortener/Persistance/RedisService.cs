using ServiceStack.Redis;
using UrlShortener.Models;

namespace UrlShortener.Persistance;

public class RedisService : IRedisService
{
    private readonly RedisManagerPool _manager;

    public RedisService()
    {
        _manager = new RedisManagerPool("localhost:6379");
    }

    public async Task InsertKeyAsync(string key, ShortenedUrl value, CancellationToken token = default)
    {
        var valueInDb = await GetValueAsync(key, token);
        if (valueInDb != null)
        {
            return;
        }

        await using var redis = await _manager.GetClientAsync(token);
        await redis.AddAsync(key, value, token);
    }

    public async Task<ShortenedUrl> GetValueAsync(string key, CancellationToken token = default)
    {
        await using var redis = await _manager.GetClientAsync(token);
        return await redis.GetAsync<ShortenedUrl>(key, token);
    }
}
