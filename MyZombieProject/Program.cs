using MyZombieProject.Services;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("openweather", client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddSingleton<AppSettingsStore>();
builder.Services.AddSingleton<ApiKeyState>();
builder.Services.AddScoped<OpenWeatherService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// ✅ Force SignalR to avoid WebSockets (LongPolling only)
app.MapBlazorHub(options =>
{
    options.Transports = HttpTransportType.LongPolling;
});

app.MapRazorComponents<MyZombieProject.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
