using System;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Core;
using GeoNRage.App.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;

namespace GeoNRage.App
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Application>("#app");

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<GeoNRageStateProvider>();
            builder.Services.AddScoped<MapStatusHandler>();
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<GeoNRageStateProvider>());

            builder.ConfigureRefitClient<IGamesApi>();
            builder.ConfigureRefitClient<IMapsApi>();
            builder.ConfigureRefitClient<IPlayersApi>();
            builder.ConfigureRefitClient<IAuthApi>();
            builder.ConfigureRefitClient<IChallengesApi>();
            builder.ConfigureRefitClient<ILocationsApi>();
            builder.ConfigureRefitClient<IAdminApi>();

            builder.Services.AddSingleton(new PopupService());
            builder.Services.AddSingleton(new MapStatusService());

            builder.Logging.SetMinimumLevel(LogLevel.Warning);

            await builder.Build().RunAsync();
        }

        private static void ConfigureRefitClient<T>(this WebAssemblyHostBuilder builder) where T : class
        {
            builder
                .Services
                .AddRefitClient<T>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<MapStatusHandler>();
        }
    }
}
