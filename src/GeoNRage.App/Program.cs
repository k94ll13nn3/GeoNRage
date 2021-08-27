using GeoNRage.App;
using GeoNRage.App.Apis;
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

builder.AddRefitClient<IGamesApi>();
builder.AddRefitClient<IMapsApi>();
builder.AddRefitClient<IPlayersApi>();
builder.AddRefitClient<IAuthApi>();
builder.AddRefitClient<IChallengesApi>();
builder.AddRefitClient<ILocationsApi>();
builder.AddRefitClient<IAdminApi>();

builder.Services.AddSingleton<PopupService>();
builder.Services.AddSingleton<UserSettingsService>();
builder.Services.AddSingleton<ToastService>();

builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();
