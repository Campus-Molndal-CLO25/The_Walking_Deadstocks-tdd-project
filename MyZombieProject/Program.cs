using MyZombieProject.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Razor Components + Interactive Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient: OpenWeather
builder.Services.AddHttpClient("openweather", client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// HttpClient: Gemini
builder.Services.AddHttpClient("gemini", client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// Local settings + state
builder.Services.AddSingleton<AppSettingsStore>();
builder.Services.AddSingleton<ApiKeyState>();

// API services
builder.Services.AddScoped<OpenWeatherService>();
builder.Services.AddScoped<GeminiService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<MyZombieProject.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
