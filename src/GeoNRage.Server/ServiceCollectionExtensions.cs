using System.Net;
using GeoNRage.Server.Bot;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Identity;
using GeoNRage.Server.Services;
using GeoNRage.Server.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway.Extensions;

namespace GeoNRage.Server;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupTasks(this IServiceCollection services)
    {
        services.AddTransient<IStartupTask, DatabaseMigrationTask>();
        services.AddTransient<IStartupTask, RoleCreationTask>();
        services.AddTransient<IStartupTask, SuperAdminCreationTask>();

        return services;
    }

    public static IServiceCollection AddDiscordBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<DiscordService>();
        services.AddHostedService<PresenceService>();

        services.AddDiscordGateway(_ => configuration[$"{nameof(ApplicationOptions)}:{nameof(ApplicationOptions.DiscordBotToken)}"]);
        services.AddDiscordCommands(true);
        services.AddCommandGroup<BotCommands>();
        services.AddAutocompleteProvider<PlayerNameAutocompleteProvider>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<GameService>();
        services.AddTransient<MapService>();
        services.AddTransient<PlayerService>();
        services.AddTransient<ChallengeService>();
        services.AddTransient<LocationService>();
        services.AddTransient<GeoGuessrService>();
        services.AddTransient<AdminService>();

        return services;
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        var cookieContainer = new CookieContainer();
        services.AddSingleton(cookieContainer);

        services.AddHttpClient("geoguessr", c => c.BaseAddress = new Uri("https://www.geoguessr.com/api/v3/"))
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };
        });

        services.AddHttpClient("google", c => c.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/"));

        return services;
    }

    public static IServiceCollection AddDatabaseAndIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("GeoNRageConnection");
        services.AddDbContextPool<GeoNRageDbContext>(options =>
            options
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning)));

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<GeoNRageDbContext>()
            .AddUserStore<GeoNRageUserStore>()
            .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider)
            .AddErrorDescriber<FrenchIdentityErrorDescriber>()
            .AddClaimsPrincipalFactory<GeoNRageUserClaimsPrincipalFactory>();

        services.AddDataProtection().PersistKeysToDbContext<GeoNRageDbContext>();
        services.Configure<KeyManagementOptions>(options => options.XmlEncryptor = new NullXmlEncryptor());

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

        return services;
    }
}
