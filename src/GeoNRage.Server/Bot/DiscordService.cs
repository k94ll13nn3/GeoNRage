using Microsoft.Extensions.Options;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Rest.Core;
using Remora.Results;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
internal sealed partial class DiscordService : BackgroundService
{
    private readonly ILogger<DiscordService> _logger;
    private readonly DiscordGatewayClient _gatewayClient;
    private readonly SlashService _slashService;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Snowflake.TryParse(_options.DiscordDevServerId, out Snowflake? guild);

        Result updateSlash = await _slashService.UpdateSlashCommandsAsync(guild, null, stoppingToken);
        if (!updateSlash.IsSuccess)
        {
            LogSlashCommandsUpdateError(updateSlash.Error.Message);
            return;
        }

        Result runResult = await _gatewayClient.RunAsync(stoppingToken);
        if (!runResult.IsSuccess && !stoppingToken.IsCancellationRequested)
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

    [LoggerMessage(6, LogLevel.Error, "Failed to update slash commands: {Reason}")]
    partial void LogSlashCommandsUpdateError(string reason);

    [LoggerMessage(7, LogLevel.Error, "Exception during gateway connection: {ExceptionMessage}")]
    partial void LogGatewayConnectionException(Exception ex, string exceptionMessage);

    [LoggerMessage(8, LogLevel.Error, "Gateway error: {Reason}")]
    partial void LogGatewayError(string reason);

    [LoggerMessage(9, LogLevel.Error, "Unknown error: {Reason}")]
    partial void LogUnknownError(string reason);
}
