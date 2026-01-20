using Microsoft.EntityFrameworkCore;
using MyZombieProject.App.Datalayer;
using MyZombieProject.App.Datalayer.Repositories;
using MyZombieProject.Services;

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

            builder.Services.AddScoped<SupplyRepository>();
            builder.Services.AddScoped<SurvivorRepository>();
            builder.Services.AddScoped<ShelterRepository>();
            builder.Services.AddScoped<DataFacade>();
            
            builder.Services.AddScoped<OpenWeatherService>();

            builder.Services.AddScoped<ApiKeyState>(); 

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
           
            builder.Services.AddHttpClient();

           
            builder.Services.AddHttpClient("openweather", client =>
            {
                client.BaseAddress = new Uri("https://api.openweathermap.org/");
            });

          
            builder.Services.AddScoped<OpenWeatherService>();
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
