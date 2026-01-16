namespace MyZombieProject.Services;

public sealed class ApiKeyState
{
    public string? OpenWeatherApiKey { get; private set; }

    public bool HasKey => !string.IsNullOrWhiteSpace(OpenWeatherApiKey);

    public void Set(string key) => OpenWeatherApiKey = key;
    public void Clear() => OpenWeatherApiKey = null;
}