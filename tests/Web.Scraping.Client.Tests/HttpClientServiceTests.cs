using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;

namespace Web.Scraping.Client.Tests
{
    public class HttpClientServiceTests
    {
        private readonly ILogger<HttpClientService> _logger;

        public HttpClientServiceTests()
        {
            _logger = Substitute.For<ILogger<HttpClientService>>();
        }

        [Fact]
        public async Task GetHtmlAsync_Should_ReturnHtmlContent_When_RequestIsSuccessful()
        {
            var handler = new MockHttpMessageHandler();
            handler.When(HttpMethod.Get, "*").Respond(HttpStatusCode.OK, "text/html", "<html></html>");

            var _httpClient = new HttpClient(handler);
            var sut = new HttpClientService(_httpClient, _logger);

            var result = await sut.GetHtmlAsync("https://example.com");

            result.Should().NotBeNull();
            result.Should().Be("<html></html>");
        }

        [Fact]
        public async Task GetHtmlAsync_Should_ReturnNull_When_RequestFails()
        {
            var handler = new MockHttpMessageHandler();
            handler.When(HttpMethod.Get, "*").Respond(HttpStatusCode.InternalServerError);
            var _httpClient = new HttpClient(handler);
            var service = new HttpClientService(_httpClient, _logger);

            var result = await service.GetHtmlAsync("https://example.com");

            result.Should().BeNull();
            _logger.Received(1).LogWarning("Unable to fetch the HTML content.");
        }

        [Fact]
        public async Task GetHtmlAsync_Should_LogError_When_UnexpectedExceptionOccurs()
        {
            var handler = new MockHttpMessageHandler();
            handler.When(HttpMethod.Get, "*").Throw(new Exception("Unexpected error"));
            var _httpClient = new HttpClient(handler);
            var service = new HttpClientService(_httpClient, _logger);

            Func<Task> act = async () => await service.GetHtmlAsync("https://example.com");

            await act.Should().ThrowAsync<ClientException>()
                     .WithMessage("An error occurred while fetching the HTML content.");

            _logger.Received(1).LogError("Unknown error: Unexpected error");
        }
    }
}
