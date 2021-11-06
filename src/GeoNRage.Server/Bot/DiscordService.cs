using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Services;
using Remora.Discord.Core;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
public partial class DiscordService : BackgroundService
{
    private readonly ILogger<DiscordService> _logger;
    private readonly DiscordGatewayClient _gatewayClient;
    private readonly SlashService _slashService;
    private readonly IDiscordRestOAuth2API _oauth2API;
    private readonly IDiscordRestApplicationAPI _discordRestApplicationAPI;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Result checkSlashSupport = _slashService.SupportsSlashCommands();
        if (!checkSlashSupport.IsSuccess)
        {
            LogSlashCommandsNotSupported(checkSlashSupport.Error.Message);
            return;
        }

        Result<IApplication> getApplication = await _oauth2API.GetCurrentBotApplicationInformationAsync(stoppingToken);
        if (!getApplication.IsSuccess)
        {
            LogApplicationFetchError(getApplication.Error.Message);
            return;
        }

        Snowflake applicationId = getApplication.Entity.ID;
        _ = Snowflake.TryParse(_options.DiscordDevServerId, out Snowflake? guild);

        Result<IReadOnlyList<IApplicationCommand>> globalCommands = await _discordRestApplicationAPI.GetGlobalApplicationCommandsAsync(applicationId, stoppingToken);
        if (globalCommands.IsSuccess)
        {
            foreach (IApplicationCommand command in globalCommands.Entity)
            {
                Result deleteResult = await _discordRestApplicationAPI.DeleteGlobalApplicationCommandAsync(applicationId, command.ID, stoppingToken);
                if (!deleteResult.IsSuccess)
                {
                    LogGlobalCommandsDeleteError(deleteResult.Error.Message);
                }
            }
        }
        else
        {
            LogGlobalCommandsGetError(globalCommands.Error.Message);
        }

        if (guild.HasValue)
        {
            Result<IReadOnlyList<IApplicationCommand>> guildCommands = await _discordRestApplicationAPI.GetGuildApplicationCommandsAsync(applicationId, guild.Value, stoppingToken);
            if (guildCommands.IsSuccess)
            {
                foreach (IApplicationCommand? guildCommand in guildCommands.Entity)
                {
                    Result deleteResult = await _discordRestApplicationAPI.DeleteGuildApplicationCommandAsync(applicationId, guild.Value, guildCommand.ID, stoppingToken);
                    if (!deleteResult.IsSuccess)
                    {
                        LogGuildCommandsDeleteError(deleteResult.Error.Message);
                    }
                }
            }
            else
            {
                LogGuildCommandsGetError(guildCommands.Error.Message);
            }
        }

        Result updateSlash = await _slashService.UpdateSlashCommandsAsync(guild, stoppingToken);
        if (!updateSlash.IsSuccess)
        {
            LogSlashCommandsUpdateError(updateSlash.Error.Message);
            return;
        }

        Result runResult = await _gatewayClient.RunAsync(stoppingToken);
        if (!runResult.IsSuccess)
        {
            switch (runResult.Error)
            {
                case ExceptionError exe:
                    LogGatewayConnectionException(exe.Exception, exe.Message);
                    break;

                case GatewayWebSocketError:
                case GatewayDiscordError:
                    LogGatewayError(runResult.Error.Message);
                    break;

                default:
                    LogUnknownError(runResult.Error.Message);
                    break;
            }
        }
    }

    [LoggerMessage(0, LogLevel.Error, "The registered commands of the bot don't support slash commands: {Reason}")]
    partial void LogSlashCommandsNotSupported(string reason);

    [LoggerMessage(1, LogLevel.Error, "Failed to get application: {Reason}")]
    partial void LogApplicationFetchError(string reason);

    [LoggerMessage(2, LogLevel.Warning, "Failed to get global commands: {Reason}")]
    partial void LogGlobalCommandsGetError(string reason);

    [LoggerMessage(3, LogLevel.Warning, "Failed to delete global commands: {Reason}")]
    partial void LogGlobalCommandsDeleteError(string reason);

    [LoggerMessage(4, LogLevel.Warning, "Failed to get guild commands: {Reason}")]
    partial void LogGuildCommandsGetError(string reason);

    [LoggerMessage(5, LogLevel.Warning, "Failed to delete guild commands: {Reason}")]
    partial void LogGuildCommandsDeleteError(string reason);

    [LoggerMessage(6, LogLevel.Error, "Failed to update slash commands: {Reason}")]
    partial void LogSlashCommandsUpdateError(string reason);

    [LoggerMessage(7, LogLevel.Error, "Exception during gateway connection: {ExceptionMessage}")]
    partial void LogGatewayConnectionException(Exception ex, string exceptionMessage);

    [LoggerMessage(8, LogLevel.Error, "Gateway error: {Reason}")]
    partial void LogGatewayError(string reason);

    [LoggerMessage(9, LogLevel.Error, "Unknown error: {Reason}")]
    partial void LogUnknownError(string reason);
}
