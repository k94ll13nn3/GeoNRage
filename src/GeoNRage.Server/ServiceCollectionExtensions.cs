using GeoNRage.Server.Bot;
using GeoNRage.Server.Tasks;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway.Extensions;

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

        services.AddDiscordGateway(_ => configuration[$"{nameof(ApplicationOptions)}:{nameof(ApplicationOptions.DiscordBotToken)}"]);
        services.AddDiscordCommands(true);
        services.AddCommandGroup<BotCommands>();

        return services;
    }
}
