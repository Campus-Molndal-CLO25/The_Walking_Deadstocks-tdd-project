using Microsoft.EntityFrameworkCore;
using MyZombieProject.App.Datalayer;
using MyZombieProject.App.Datalayer.Repositories;
using MyZombieProject.Services;
using System.Net.Http.Headers;

namespace MyZombieProject.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MyZombieDataContext>(options =>
                options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<ISupplyRepository, SupplyRepository>();
            builder.Services.AddScoped<ISurvivorRepository, SurvivorRepository>();
            builder.Services.AddScoped<IShelterRepository, ShelterRepository>();
            builder.Services.AddScoped<DataFacade>();

            builder.Services.AddSingleton<MissionService>();
            builder.Services.AddSingleton<AppSettingsStore>();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
           
            builder.Services.AddHttpClient();


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


            builder.Services.AddScoped<OpenWeatherService>();
            builder.Services.AddScoped<GeminiService>();
            builder.Services.AddScoped<ApiKeyState>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<Components.App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
