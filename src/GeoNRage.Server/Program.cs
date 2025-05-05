using GeoNRage.Server;
using GeoNRage.Server.Endpoints;
using GeoNRage.Server.Hubs;
using GeoNRage.Server.Tasks;
using GeoNRage.ServiceDefaults;
using Microsoft.AspNetCore.ResponseCompression;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddLogging();
}
else
{
    builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSeq(builder.Configuration.GetSection("Seq")));
}

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(nameof(ApplicationOptions)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]));
builder.Services.AddMemoryCache(); // SizeLimit cannot be specified because Remora uses the same cache without specifying an entity size :(.
builder.Services.AddDatabaseAndIdentity(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddHttpClients();
builder.Services.AddDiscordBot(builder.Configuration);
builder.Services.AddStartupTasks();

builder.EnrichMySqlDbContext<GeoNRageDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30;
    });

builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
}

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseHttpsRedirection();
}
else
{
    // Prevent Hot reload if used in dev, see https://github.com/dotnet/aspnetcore/issues/28293.
    app.UseResponseCompression();
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapApplicationEndpoints();
app.MapDefaultEndpoints();

app.MapHub<AppHub>("/apphub");

app.MapStaticAssets();
app.MapRazorComponents<Application>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GeoNRage.App._Imports).Assembly);

ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    foreach (IStartupTask startupTask in app.Services.GetServices<IStartupTask>())
    {
        await startupTask.ExecuteAsync();
    }

    await app.RunAsync();
}
catch (Exception exception)
{
    Loggers.LogUnhandledException(logger, "Stopped server because of exception", exception);
    throw;
}
