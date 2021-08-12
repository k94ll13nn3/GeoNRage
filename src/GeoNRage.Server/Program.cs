using System.Net;
using GeoNRage.Server;
using GeoNRage.Server.Hubs;
using GeoNRage.Server.Services;
using GeoNRage.Server.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NLog;
using NLog.Web;

Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(nameof(ApplicationOptions)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSignalR();
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddRazorPages();
builder.Services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));

string connectionString = builder.Configuration.GetConnectionString("GeoNRageConnection");
builder.Services.AddDbContextPool<GeoNRageDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<GeoNRageDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

builder.Services.AddTransient<GameService>();
builder.Services.AddTransient<MapService>();
builder.Services.AddTransient<PlayerService>();
builder.Services.AddTransient<ChallengeService>();
builder.Services.AddTransient<LocationService>();
builder.Services.AddTransient<GeoGuessrService>();
builder.Services.AddTransient<AdminService>();

var cookieContainer = new CookieContainer();
builder.Services.AddSingleton(cookieContainer);

builder.Services.AddHttpClient("geoguessr", c => c.BaseAddress = new Uri("https://www.geoguessr.com/api/v3/"))
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler()
        {
            CookieContainer = cookieContainer
        };
    });

builder.Services.AddHttpClient("google", c => c.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/"));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

builder.Services.AddStartupTask<DatabaseMigrationTask>();
builder.Services.AddStartupTask<RoleCreationTask>();
builder.Services.AddStartupTask<SuperAdminCreationTask>();

WebApplication app = builder.Build();

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Geo N'Rage API"));
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GameMetadataMiddleware>();

app.MapRazorPages();
app.MapControllers();
app.MapHub<AppHub>("/apphub");
app.MapFallbackToFile("index.html");

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
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
