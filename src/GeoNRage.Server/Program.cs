using GeoNRage.Server;
using GeoNRage.Server.Endpoints;
using GeoNRage.Server.Hubs;
using GeoNRage.Server.Tasks;
using Microsoft.AspNetCore.ResponseCompression;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSeq(builder.Configuration.GetSection("Seq")));

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(nameof(ApplicationOptions)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));
builder.Services.AddMemoryCache(); // SizeLimit cannot be specified because Remora uses the same cache without specifing an entity size :(.
builder.Services.AddDatabaseAndIdentity(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddHttpClients();
builder.Services.AddDiscordBot(builder.Configuration);
builder.Services.AddStartupTasks();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();
}

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Geo N'Rage API");
        c.DisplayRequestDuration();
    });
    app.UseHttpsRedirection();
}
else
{
    // Prevent Hot reload if used in dev, see https://github.com/dotnet/aspnetcore/issues/28293.
    app.UseResponseCompression();
    app.UseHsts();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapApplicationEndpoints();
app.MapHub<AppHub>("/apphub");
app.MapFallbackToFile("index.html");

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
