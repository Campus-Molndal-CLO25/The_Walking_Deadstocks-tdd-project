using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyZombieProject.Services;
using Xunit;

public class GeminiServiceTests
{
    [Fact]
    public async Task AskSurvivalAdvisorAsync_WhenApiKeyIsMissing_ReturnsErrorMessage()
    {
        // Arrange
        var service = CreateServiceWithHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await service.AskSurvivalAdvisorAsync("Hej", "");

        // Assert
        Assert.Equal("Gemini API key saknas (GeminiApiKey i settings.json).", result);
    }

    [Fact]
    public async Task AskSurvivalAdvisorAsync_WhenQuestionIsMissing_ReturnsErrorMessage()
    {
        // Arrange
        var service = CreateServiceWithHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await service.AskSurvivalAdvisorAsync("   ", "apikey");

        // Assert
        Assert.Equal("Du måste skriva en fråga.", result);
    }

    [Fact]
    public async Task AskSurvivalAdvisorAsync_WhenGenerateContentReturns401_ReturnsGeminiApiError()
    {
        // Arrange
        var service = CreateServiceWithHandler(request =>
        {
            var url = request.RequestUri?.ToString() ?? "";

            // 1) List models
            if (request.Method == HttpMethod.Get && url.EndsWith("v1beta/models"))
            {
                return Json(HttpStatusCode.OK,
                    """
                    {
                      "models": [
                        {
                          "name": "models/gemini-1.5-flash",
                          "supportedGenerationMethods": ["generateContent"]
                        }
                      ]
                    }
                    """
                );
            }

            // 2) generateContent -> 401
            if (request.Method == HttpMethod.Post && url.Contains(":generateContent"))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Invalid key", Encoding.UTF8, "text/plain")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        // Act
        var result = await service.AskSurvivalAdvisorAsync("Test", "bad-key");

        // Assert
        Assert.Contains("Gemini API error (401):", result);
        Assert.Contains("Invalid key", result);
    }

    [Fact]
    public async Task AskSurvivalAdvisorAsync_WhenApiReturnsValidLongText_ReturnsFormattedAnswer()
    {
        // Arrange
        var service = CreateServiceWithHandler(request =>
        {
            var url = request.RequestUri?.ToString() ?? "";

            if (request.Method == HttpMethod.Get && url.EndsWith("v1beta/models"))
            {
                return Json(HttpStatusCode.OK,
                    """
                    {
                      "models": [
                        {
                          "name": "models/gemini-1.5-flash",
                          "supportedGenerationMethods": ["generateContent"]
                        }
                      ]
                    }
                    """
                );
            }

            if (request.Method == HttpMethod.Post && url.Contains(":generateContent"))
            {
                // >700 tecken + [SLUT] så att din service INTE gör "continue"-anrop
                var longText = new string('A', 710) + " [SLUT]";

                // Bygg JSON utan raw-interpolation som bråkar med { }
                var json = """
                {
                  "candidates": [
                    {
                      "finishReason": "STOP",
                      "content": {
                        "parts": [
                          { "text": "__TEXT__" }
                        ]
                      }
                    }
                  ],
                  "promptFeedback": { "blockReason": "none" }
                }
                """.Replace("__TEXT__", EscapeForJson(longText));

                return Json(HttpStatusCode.OK, json);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        // Act
        var result = await service.AskSurvivalAdvisorAsync("Hur överlever jag?", "key");

        // Assert
        Assert.Contains("(Model: models/gemini-1.5-flash)", result);
        Assert.Contains("finishReason=STOP", result);
        Assert.Contains("blockReason=none", result);
        Assert.Contains("[SLUT]", result);
    }

    [Fact]
    public async Task AskSurvivalAdvisorAsync_WhenApiReturnsNoText_ReturnsIngetSvarMessage()
    {
        // Arrange
        var service = CreateServiceWithHandler(request =>
        {
            var url = request.RequestUri?.ToString() ?? "";

            if (request.Method == HttpMethod.Get && url.EndsWith("v1beta/models"))
            {
                return Json(HttpStatusCode.OK,
                    """
                    {
                      "models": [
                        {
                          "name": "models/gemini-1.5-flash",
                          "supportedGenerationMethods": ["generateContent"]
                        }
                      ]
                    }
                    """
                );
            }

            if (request.Method == HttpMethod.Post && url.Contains(":generateContent"))
            {
                return Json(HttpStatusCode.OK,
                    """
                    {
                      "candidates": [
                        {
                          "finishReason": "STOP",
                          "content": { "parts": [] }
                        }
                      ],
                      "promptFeedback": { "blockReason": "none" }
                    }
                    """
                );
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        // Act
        var result = await service.AskSurvivalAdvisorAsync("Hej", "key");

        // Assert
        Assert.Contains("Inget svar returnerades från Gemini.", result);
        Assert.Contains("finishReason=STOP", result);
        Assert.Contains("blockReason=none", result);
    }

    [Fact]
    public async Task AskSurvivalAdvisorAsync_WhenHttpThrows_ReturnsUnexpectedError()
    {
        // Arrange
        var service = CreateServiceWithHandler(_ => throw new HttpRequestException("Network failed"));

        // Act
        var result = await service.AskSurvivalAdvisorAsync(
            userQuestion: "Hej",
            apiKey: "key",
            preferredModelFromSettings: "models/gemini-1.5-flash"
        );

        // Assert
        Assert.Contains("Unexpected error:", result);
        Assert.Contains("Network failed", result);
    }

    // ---------------------------
    // Helpers
    // ---------------------------

    private static GeminiService CreateServiceWithHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        var handler = new FakeHandler(responder);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://fake.api/")
        };

        return new GeminiService(new FakeHttpClientFactory(client));
    }

    private static HttpResponseMessage Json(HttpStatusCode code, string json)
    {
        return new HttpResponseMessage(code)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    // Minimal JSON escaping för textfältet
    private static string EscapeForJson(string value)
        => value.Replace("\\", "\\\\").Replace("\"", "\\\"");

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
