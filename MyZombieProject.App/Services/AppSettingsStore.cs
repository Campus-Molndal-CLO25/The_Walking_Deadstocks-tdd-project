using System.Text.Json;

namespace MyZombieProject.Services;

public sealed class AppSettingsStore
{
    private const string AppName = "MyZombieProject";
    private const string FileName = "settings.json";

    private string SettingsPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            AppName,
            FileName);

    public async Task<AppSettings> LoadAsync()
    {
        if (!File.Exists(SettingsPath))
            return new AppSettings();

        var json = await File.ReadAllTextAsync(SettingsPath);

        // Lägg till dessa options för att ignorera stora/små bokstäver!
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<AppSettings>(json, options)
               ?? new AppSettings();
    }

    public async Task SaveAsync(AppSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);

        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(SettingsPath, json);
    }
}



public sealed class AppSettings
{
    public string? OpenWeatherApiKey { get; set; }
    public string? GeminiApiKey { get; set; }

    public string? GeminiModel { get; set; }
}