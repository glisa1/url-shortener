using ServiceStack.Redis;
using UrlShortener.Models;

namespace UrlShortener.Persistence;

public class RedisService
{
    private readonly RedisManagerPool _manager;

    //ADD TO DI!
    public RedisService()
    {
        _manager = new RedisManagerPool("localhost:6379");
    }

    public void InsertKey(string key, ShortenedUrl value)
    {
        using var redis = _manager.GetClient();
        redis.Add(key, value);
    }

    public ShortenedUrl GetValue(string key)
    {
        using var redis = _manager.GetClient();
        return redis.Get<ShortenedUrl>(key);
    }
}
