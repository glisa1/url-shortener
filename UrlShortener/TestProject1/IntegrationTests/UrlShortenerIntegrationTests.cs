using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.Redis;
using UrlShortener.Models;
using UrlShortener.Persistance;

namespace UrlShortener.Tests.IntegrationTests
{
    public class UrlShortenerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncDisposable
    {
        private readonly WebApplicationFactory<Program> _builder;

        private const int DefaultRedisPort = 6379;

        private readonly RedisContainer redisTestInstance;

        public UrlShortenerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            redisTestInstance = new RedisBuilder()
                .WithPortBinding(DefaultRedisPort, true)
                .Build();

            redisTestInstance.StartAsync().Wait();

            IPersistanceService redisService = new RedisService(
                string.Format($"{redisTestInstance.Hostname}:{{0}}",
                redisTestInstance.GetMappedPublicPort(DefaultRedisPort)
                ));

            _builder = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(redisService);
                });
            });
        }

        [Fact]
        public async Task OnGettingNonExistingUrl_Fails_UrlNotFound()
        {
            const string url = "http://non-existing.com";
            var client = _builder.CreateClient();

            var response = await client.GetAsync($"/shortenedurl?url={url}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ShorteningUrl_Passes_UrlShortenedAndPersisted()
        {
            const string url = "http://url-shortener-integration-test.com";
            var client = _builder.CreateClient();

            var postResponse = await client.PostAsJsonAsync("/shortenurl", new ShortenUrlRequest(url));
            var postResponseObject = await postResponse.Content.ReadAsStringAsync();

            var shortenedUrl = JsonConvert.DeserializeObject<ShortenedUrl>(postResponseObject);

            await client.GetAsync($"/shortenedurl?url={shortenedUrl!.ShortUrl}");

            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
            Assert.NotNull(shortenedUrl);
            Assert.Equal(url, shortenedUrl.LongUrl);
        }

        public ValueTask DisposeAsync() => redisTestInstance.DisposeAsync();
    }
}
