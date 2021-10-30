using GeoNRage.Server.Bot;
using GeoNRage.Server.Tasks;
using Remora.Commands.Extensions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.API.Objects;

namespace GeoNRage.Server;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
        where T : class, IStartupTask
    {
        return services.AddTransient<IStartupTask, T>();
    }

    public static IServiceCollection AddDiscordBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<DiscordService>();
        services.AddHostedService<PresenceService>();

        services.AddDiscordGateway(_ => configuration[$"{nameof(ApplicationOptions)}:{nameof(ApplicationOptions.DiscordBotToken)}"]);
        services.AddDiscordCommands(true);
        services.AddCommandGroup<BotCommands>();
        services.AddAutocompleteProvider<PlayerNameAutocompleteProvider>();

        services.Configure<DiscordGatewayClientOptions>(opt => opt.Presence = new UpdatePresence(
            ClientStatus.Online,
            false,
            null,
            new[] { new Activity("GeoGuessr", ActivityType.Game) }));

        return services;
    }
}
