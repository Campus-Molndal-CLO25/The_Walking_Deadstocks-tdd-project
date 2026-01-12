using MyZombieProject.Components;
using MyZombieProject.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("openweather", client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// 🔽 SERVICES (MERGED + FIXED)
builder.Services.AddSingleton<AppSettingsStore>();
builder.Services.AddSingleton<ApiKeyState>();      // ✅ DEN HÄR RADEN
builder.Services.AddScoped<OpenWeatherService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
