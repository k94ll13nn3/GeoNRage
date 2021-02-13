using System;
using System.Threading.Tasks;
using GeoNRage.App.Clients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GeoNRage.App
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient<GamesHttpClient>(client => client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/games"));

            await builder.Build().RunAsync();
        }
    }
}
