using System.Net.Http.Json;

namespace MyZombieProject.Services;

public sealed class OpenWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OpenWeatherService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ForecastResult> GetForecastAsync(string city, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(city))
            return ForecastResult.Fail("City is required.");

        if (string.IsNullOrWhiteSpace(apiKey))
            return ForecastResult.Fail("API key is required.");

        var http = _httpClientFactory.CreateClient("openweather");

        var url =
            $"data/2.5/forecast" +
            $"?q={Uri.EscapeDataString(city)}" +
            $"&appid={Uri.EscapeDataString(apiKey)}" +
            $"&units=metric&lang=sv";

        try
        {
            var data = await http.GetFromJsonAsync<OpenWeatherForecastResponse>(url);

            if (data?.List == null || data.List.Length == 0)
                return ForecastResult.Fail("No forecast data returned.");

            var first = data.List[0];
            var desc = first.Weather?.FirstOrDefault()?.Description ?? "n/a";

            var message =
                $"{data.City?.Name ?? city}: " +
                $"{first.DtTxt} → {first.Main?.Temp}°C, {desc}";

            return ForecastResult.Ok(message);
        }
        catch (HttpRequestException ex)
        {
            return ForecastResult.Fail($"HTTP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ForecastResult.Fail($"Unexpected error: {ex.Message}");
        }
    }

    // DTOs (minimala)
    private sealed class OpenWeatherForecastResponse
    {
        public CityDto? City { get; set; }
        public ForecastItemDto[]? List { get; set; }
    }

    private sealed class CityDto
    {
        public string? Name { get; set; }
    }

    private sealed class ForecastItemDto
    {
        public MainDto? Main { get; set; }
        public WeatherDto[]? Weather { get; set; }
        public string? DtTxt { get; set; }
    }

    private sealed class MainDto
    {
        public double? Temp { get; set; }
    }

    private sealed class WeatherDto
    {
        public string? Description { get; set; }
    }
}

public readonly record struct ForecastResult(bool Success, string Message)
{
    public static ForecastResult Ok(string message) => new(true, message);
    public static ForecastResult Fail(string message) => new(false, message);
}