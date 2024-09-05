using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.Redis;
using UrlShortener.Persistance;
using Xunit;

namespace UrlShortener.Test
{
    [TestClass]
    public class ApiEndpointTests : IAsyncLifetime, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> factory;

        private readonly Mock<IRedisService> redisServiceMock = new();

        private const int DefaultRedisPort = 6379;

        private readonly IContainer _redisTestInstance = new RedisBuilder()
            .WithImage("latest")
            .WithName("redis-test-container")
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

        [TestMethod]
        public async void OnShorteningUrl_Fails_PassedStringNotAnUrl()
        {
            var httpClient = factory.CreateClient();

            var response = await httpClient.PostAsJsonAsync("/shortenurl", new { url = "notAValidUrl" });

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }
    }
}