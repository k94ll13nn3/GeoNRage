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
            _logger.LogWarning
            (
                "The registered commands of the bot don't support slash commands: {Reason}",
                checkSlashSupport.Error.Message
            );

            return;
        }

        Result<IApplication> getApplication = await _oauth2API.GetCurrentBotApplicationInformationAsync(stoppingToken);
        if (!getApplication.IsSuccess)
        {
            _logger.LogWarning("Failed to get application: {Reason}", getApplication.Error.Message);
            return;
        }

        Snowflake applicationId = getApplication.Entity.ID;

        if (!Snowflake.TryParse(_options.DiscordDevServerId, out Snowflake? guild))
        {
            _logger.LogWarning($"Failed to parse '{nameof(ApplicationOptions.DiscordDevServerId)}'");
            return;
        }

        Result<IReadOnlyList<IApplicationCommand>> globalCommands = await _discordRestApplicationAPI.GetGlobalApplicationCommandsAsync(applicationId, stoppingToken);
        foreach (IApplicationCommand command in globalCommands.Entity)
        {
            Result deleteResult = await _discordRestApplicationAPI.DeleteGlobalApplicationCommandAsync(applicationId, command.ID, stoppingToken);
            if (!deleteResult.IsSuccess)
            {
                _logger.LogWarning("Failed to delete global command: {Reason}", deleteResult.Error.Message);
            }
        }

        Result<IReadOnlyList<IApplicationCommand>> guildCommands = await _discordRestApplicationAPI.GetGuildApplicationCommandsAsync(applicationId, guild.Value, stoppingToken);
        foreach (IApplicationCommand? guildCommand in guildCommands.Entity)
        {
            Result deleteResult = await _discordRestApplicationAPI.DeleteGuildApplicationCommandAsync(applicationId, guild.Value, guildCommand.ID, stoppingToken);
            if (!deleteResult.IsSuccess)
            {
                _logger.LogWarning("Failed to delete global command: {Reason}", deleteResult.Error.Message);
            }
        }

        Result updateSlash = await _slashService.UpdateSlashCommandsAsync(guild, stoppingToken);
        if (!updateSlash.IsSuccess)
        {
            _logger.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
            return;
        }

        Result runResult = await _gatewayClient.RunAsync(stoppingToken);
        if (!runResult.IsSuccess)
        {
            switch (runResult.Error)
            {
                case ExceptionError exe:
                    _logger.LogError(exe.Exception, "Exception during gateway connection: {ExceptionMessage}", exe.Message);
                    break;

                case GatewayWebSocketError:
                case GatewayDiscordError:
                    _logger.LogError("Gateway error: {Message}", runResult.Error.Message);
                    break;

                default:
                    _logger.LogError("Unknown error: {Message}", runResult.Error.Message);
                    break;
            }
        }
    }
}
