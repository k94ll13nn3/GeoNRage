using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// TODO: MapStaticAssets

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
builder.AddRefitClient<ILogApi>();

builder.Services.AddSingleton<UserSettingsService>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddSingleton<ModalService>();

builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.Services.TryAddEnumerable(
    ServiceDescriptor.Singleton<ILoggerProvider, UnhandledExceptionLoggerProvider>());

WebAssemblyHost host = builder.Build();
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

AppDomain.CurrentDomain.UnhandledException += (_, a) => Loggers.LogUnhandledException(logger, a.ExceptionObject.ToString()!, (a.ExceptionObject as Exception)!);

try
{
    await host.RunAsync();
}
catch (Exception exception)
{
    Loggers.LogUnhandledException(logger, "Stopped app because of exception", exception);
    throw;
}
