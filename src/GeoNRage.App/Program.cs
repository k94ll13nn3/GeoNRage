using GeoNRage.App;
using GeoNRage.App.Apis;
using GeoNRage.App.Core;
using GeoNRage.App.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Application>("#app");

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<GeoNRageStateProvider>();
builder.Services.AddScoped<MapStatusHandler>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<GeoNRageStateProvider>());

ConfigureRefitClient<IGamesApi>(builder);
ConfigureRefitClient<IMapsApi>(builder);
ConfigureRefitClient<IPlayersApi>(builder);
ConfigureRefitClient<IAuthApi>(builder);
ConfigureRefitClient<IChallengesApi>(builder);
ConfigureRefitClient<ILocationsApi>(builder);
ConfigureRefitClient<IAdminApi>(builder);

builder.Services.AddSingleton(new PopupService());
builder.Services.AddSingleton(new MapStatusService());
builder.Services.AddSingleton(new ToastService());

builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();

static void ConfigureRefitClient<T>(WebAssemblyHostBuilder builder) where T : class
{
    builder
        .Services
        .AddRefitClient<T>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
        .AddHttpMessageHandler<MapStatusHandler>();
}
