using System.Text.Json;

namespace MyZombieProject.Services;

public sealed class AppSettingsStore
{
    private const string Company = "CampusMolndal";
    private const string AppName = "MyZombieProject";
    private const string FileName = "settings.json";

    private string SettingsPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Company,
            AppName,
            FileName);

    public async Task<AppSettings> LoadAsync()
    {
        if (!File.Exists(SettingsPath))
            return new AppSettings();

        var json = await File.ReadAllTextAsync(SettingsPath);
        return JsonSerializer.Deserialize<AppSettings>(json)
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
    // Placeholder – sparas lokalt, committas aldrig
    public string? OpenWeatherApiKey { get; set; }
}
