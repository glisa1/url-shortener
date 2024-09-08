using DotNet.Testcontainers.Containers;
using Testcontainers.Redis;
using UrlShortener.Models;
using UrlShortener.Persistance;

namespace TestProject1
{
    public class RedisServiceTests : IClassFixture<RedisServiceTestsFixture>
    {
        private readonly RedisServiceTestsFixture _fixture;
        public RedisServiceTests(RedisServiceTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void OnGettingANonExistingValue_Passes_ValueIsNull()
        {
            var value = await _fixture._redisService.GetValueAsync("test");

            Assert.Null(value);
        }

        [Fact]
        public async void OnGettingExistingValue_Passes_ExistingValueIsReturned()
        {
            var shortenedUrl = new ShortenedUrl("existing_shortened_value", "long_url", "short_url");
            await _fixture._redisService.InsertKeyAsync(shortenedUrl.Key, shortenedUrl);
            var valueRecieved = await _fixture._redisService.GetValueAsync(shortenedUrl.Key);

            Assert.Equal(shortenedUrl, valueRecieved);
        }
    }

    public class RedisServiceTestsFixture : IAsyncDisposable
    {
        private const int DefaultRedisPort = 6379;

        private readonly IContainer redisTestInstance;
        public readonly IRedisService _redisService;

        public RedisServiceTestsFixture()
        {
            redisTestInstance = new RedisBuilder()
                .WithName("redis-test-container")
                .WithPortBinding(DefaultRedisPort, true)
                .Build();

            redisTestInstance.StartAsync().Wait();

            _redisService = new RedisService(string.Format($"{redisTestInstance.Hostname}:{{0}}",
                redisTestInstance.GetMappedPublicPort(DefaultRedisPort)
                ));
        }

        ValueTask IAsyncDisposable.DisposeAsync() => redisTestInstance.DisposeAsync();
    }
}