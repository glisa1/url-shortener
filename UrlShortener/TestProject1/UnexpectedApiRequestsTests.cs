using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ServiceStack;
using System.Net;
using System.Net.Http.Json;
using UrlShortener.Models;
using UrlShortener.Persistance;

namespace UrlShortener.Tests
{
    public class UnexpectedApiRequestsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Mock<IPersistanceService> _persisanceMock = new();
        private readonly WebApplicationFactory<Program> _builder;

        private const string URL_IN_WRONG_FORMAT = "Input string is not a valid url value.";
        private const string URL_IS_EMPTY = "Url was empty.";

        public UnexpectedApiRequestsTests(WebApplicationFactory<Program> factory)
        {
            _builder = factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddSingleton(_persisanceMock.Object);
                    });
                });
        }

        #region POST

        [Fact]
        public async Task OnSendingPostRequest_Fails_UrlIsNotValid()
        {
            var client = _builder.CreateClient();

            var response = await client.PostAsJsonAsync("/shortenurl", new ShortenUrlRequest("invalid"));
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IN_WRONG_FORMAT, responseContent);
        }

        [Fact]
        public async Task OnSendingPostRequest_Fails_UrlIsEmpty()
        {
            var client = _builder.CreateClient();

            var response = await client.PostAsJsonAsync("/shortenurl", new ShortenUrlRequest(string.Empty));
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IN_WRONG_FORMAT, responseContent);
        }

        [Fact]
        public async Task OnSendingPostRequest_Fails_UrlIsOnlyWhitespace()
        {
            var client = _builder.CreateClient();

            var response = await client.PostAsJsonAsync("/shortenurl", new ShortenUrlRequest(" "));
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IN_WRONG_FORMAT, responseContent);
        }

        #endregion

        #region GET

        [Fact]
        public async Task OnSendingGetRequest_Fails_UrlIsEmpty()
        {
            var client = _builder.CreateClient();

            var response = await client.GetAsync("/shortenedurl?url=");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IS_EMPTY, responseContent);
        }

        [Fact]
        public async Task OnSendingGetRequest_Fails_UrlIsNotInRightFormat()
        {
            var client = _builder.CreateClient();

            var response = await client.GetAsync("/shortenedurl?url=test");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IN_WRONG_FORMAT, responseContent);
        }

        [Fact]
        public async Task OnSendingGetRequest_Fails_UrlIsWhitespace()
        {
            var client = _builder.CreateClient();

            var response = await client.GetAsync("/shortenedurl?url= ");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IS_EMPTY, responseContent);
        }

        [Fact]
        public async Task OnSendingGetRequest_Fails_UrlNotFound()
        {
            var client = _builder.CreateClient();

            var response = await client.GetAsync("/shortenedurl?url=http://test.com");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task OnSendingDeleteRequest_Fails_UrlIsEmpty()
        {
            var client = _builder.CreateClient();

            var response = await client.DeleteAsync("/deleteshortenedurl?url=");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IS_EMPTY, responseContent);
        }

        [Fact]
        public async Task OnSendingDeleteRequest_Fails_UrlIsNotInRightFormat()
        {
            var client = _builder.CreateClient();

            var response = await client.DeleteAsync("/deleteshortenedurl?url=test");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IN_WRONG_FORMAT, responseContent);
        }

        [Fact]
        public async Task OnSendingDeleteRequest_Fails_UrlIsWhitespace()
        {
            var client = _builder.CreateClient();

            var response = await client.DeleteAsync("/deleteshortenedurl?url= ");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(URL_IS_EMPTY, responseContent);
        }

        [Fact]
        public async Task OnSendingDeleteRequest_Fails_UrlNotFound()
        {
            var client = _builder.CreateClient();

            var response = await client.DeleteAsync("/deleteshortenedurl?url=http://test.com");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion
    }
}
