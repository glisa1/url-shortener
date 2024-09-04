using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Testcontainers.Redis;
using UrlShortener.Persistance;
using Xunit;

namespace UrlShortener.Tests
{
    public sealed class ApiEndpointTests : IAsyncLifetime, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> factory;

        private readonly Mock<IRedisService> redisServiceMock = new();

        private const int DefaultRedisPort = 6379;

        private readonly IContainer _redisTestInstance = new RedisBuilder()
            .WithImage("latest")
            .WithPortBinding(DefaultRedisPort)
            .Build();

        public ApiEndpointTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(webHostBuilder =>
            {
                webHostBuilder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IRedisService>(
                        new RedisService(
                            string.Format("http://127.0.0.1:{0}", 
                            _redisTestInstance.GetMappedPublicPort(DefaultRedisPort)
                            )
                        )
                    );
                });
            });
        }

        public Task DisposeAsync() => _redisTestInstance.StopAsync();

        public Task InitializeAsync() => _redisTestInstance.StartAsync();

        [Fact]
        public void Test1()
        {

        }
    }
}
