using MyZombieProject.Services;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Connections;
using MyZombieProject.Components;

namespace MyZombieProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("openweather", client =>
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddSingleton<AppSettingsStore>();
builder.Services.AddSingleton<ApiKeyState>();
builder.Services.AddScoped<OpenWeatherService>();

var app = builder.Build();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAntiforgery();

// ✅ Force SignalR to avoid WebSockets (LongPolling only)
app.MapBlazorHub(options =>
{
    options.Transports = HttpTransportType.LongPolling;
});

app.MapRazorComponents<MyZombieProject.Components.App>()
            app.MapStaticAssets();
            app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
        }
    }
}
