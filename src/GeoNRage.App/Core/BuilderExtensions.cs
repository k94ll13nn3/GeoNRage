using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;

namespace GeoNRage.App.Core;

public static class BuilderExtensions
{
    public static void AddRefitClient<T>(this WebAssemblyHostBuilder builder) where T : class
    {
        builder
            .Services
            .AddRefitClient<T>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            .AddHttpMessageHandler<MapStatusHandler>();
    }
}
