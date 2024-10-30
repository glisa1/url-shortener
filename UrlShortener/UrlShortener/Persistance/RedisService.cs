using ServiceStack.Redis;
using UrlShortener.Models;

namespace UrlShortener.Persistance;

public class RedisService : IPersistanceService
{
    private readonly RedisManagerPool _manager;

    public RedisService(string redisAddress = "redisinstance:6379")
    {
        _manager = new RedisManagerPool(redisAddress);
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

    public async Task DeleteValueAsync(string key, CancellationToken token = default)
    {
        await using var redis = await _manager.GetClientAsync(token);
        await redis.RemoveAsync(key, token);
    }
}
