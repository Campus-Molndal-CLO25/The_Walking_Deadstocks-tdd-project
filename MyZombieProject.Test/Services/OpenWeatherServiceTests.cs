using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyZombieProject.Services;
using Xunit;

public class OpenWeatherServiceTests
{
    [Fact]
    public async Task GetForecastAsync_WhenCityIsMissing_ReturnsFail()
    {
        // Arrange
        var service = CreateServiceWithHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await service.GetForecastAsync("", "apikey");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("City is required.", result.Message);
    }

    [Fact]
    public async Task GetForecastAsync_WhenApiKeyIsMissing_ReturnsFail()
    {
        // Arrange
        var service = CreateServiceWithHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await service.GetForecastAsync("Göteborg", "");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("API key is required.", result.Message);
    }

    [Fact]
    public async Task GetForecastAsync_WhenNoForecastListReturned_ReturnsFail()
    {
        // Arrange
        var service = CreateServiceWithHandler(request =>
        {
            // list är tom => Fail("No forecast data returned.")
            return Json(HttpStatusCode.OK, """
            {
              "city": { "name": "Göteborg" },
              "list": []
            }
            """);
        });

        // Act
        var result = await service.GetForecastAsync("Göteborg", "key");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("No forecast data returned.", result.Message);
    }


    [Fact]
    public async Task GetForecastAsync_WhenForecastDataExists_ReturnsOkMessage()
    {
        // Arrange
        var service = CreateServiceWithHandler(_ =>
        {
            return Json(HttpStatusCode.OK, """
        {
          "city": { "name": "Göteborg" },
          "list": [
            {
              "dtTxt": "2026-01-23 12:00:00",
              "main": { "temp": 5.5 },
              "weather": [ { "description": "lätt molnighet" } ]
            }
          ]
        }
        """);
        });

        // Act
        var result = await service.GetForecastAsync("Göteborg", "key");

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Göteborg:", result.Message);
        Assert.Contains("2026-01-23 12:00:00", result.Message);

        // svensk decimal (5,5) eller punkt (5.5)
        Assert.True(result.Message.Contains("5,5°C") || result.Message.Contains("5.5°C"));

        Assert.Contains("lätt molnighet", result.Message);
    }


    [Fact]
    public async Task GetForecastAsync_WhenWeatherDescriptionMissing_UsesNA()
    {
        // Arrange
        var service = CreateServiceWithHandler(request =>
        {
            // weather saknas => desc blir "n/a"
            return Json(HttpStatusCode.OK, """
            {
              "city": { "name": "Stockholm" },
              "list": [
                {
                  "dtTxt": "2026-01-23 12:00:00",
                  "main": { "temp": 1.0 }
                }
              ]
            }
            """);
        });

        // Act
        var result = await service.GetForecastAsync("Stockholm", "key");

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Stockholm:", result.Message);
        Assert.Contains("1°C", result.Message);
        Assert.Contains("n/a", result.Message);
    }

    [Fact]
    public async Task GetForecastAsync_WhenHttpRequestExceptionThrown_ReturnsHttpErrorFail()
    {
        // Arrange
        var service = CreateServiceWithHandler(_ =>
        {
            throw new HttpRequestException("Network failed");
        });

        // Act
        var result = await service.GetForecastAsync("Göteborg", "key");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("HTTP error:", result.Message);
        Assert.Contains("Network failed", result.Message);
    }

    [Fact]
    public async Task GetForecastAsync_WhenUnexpectedExceptionThrown_ReturnsUnexpectedErrorFail()
    {
        // Arrange
        var service = CreateServiceWithHandler(_ =>
        {
            throw new InvalidOperationException("Boom");
        });

        // Act
        var result = await service.GetForecastAsync("Göteborg", "key");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Unexpected error:", result.Message);
        Assert.Contains("Boom", result.Message);
    }

    // ---------------------------
    // Helpers (enkla & lokala)
    // ---------------------------

    private static OpenWeatherService CreateServiceWithHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        var handler = new FakeHandler(responder);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://fake.api/")
        };

        return new OpenWeatherService(new FakeHttpClientFactory(client));
    }

    private static HttpResponseMessage Json(HttpStatusCode code, string json)
    {
        return new HttpResponseMessage(code)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_responder(request));
    }

    private sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _client;

        public FakeHttpClientFactory(HttpClient client)
        {
            _client = client;
        }

        public HttpClient CreateClient(string name) => _client;
    }
}
