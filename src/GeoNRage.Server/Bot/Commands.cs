using System.ComponentModel;
using GeoNRage.Server.Services;
using Microsoft.Extensions.Options;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Gateway;
using Remora.Results;
using IRemoraResult = Remora.Results.IResult;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
public partial class BotCommands : CommandGroup
{
    private readonly IDiscordRestInteractionAPI _interactionApi;
    private readonly InteractionContext _interactionContext;
    private readonly DiscordGatewayClient _gatewayClient;
    private readonly GameService _gameService;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    [Command("activity")]
    [Description("Change l'activité du bot")]
    [RequireOwner]
    [Ephemeral]
    public async Task<IRemoraResult> SetActivityAsync(
        [Description("Le type d'activité")] ActivityType activity,
        [Description("Le nom de l'activité")] string name)
    {
        _gatewayClient.SubmitCommand(new UpdatePresence(
            ClientStatus.Online,
            false,
            null,
            new[] { new Activity(name, activity) }));

        Result<IMessage> reply = await _interactionApi.CreateFollowupMessageAsync(
            _interactionContext.ApplicationID,
            _interactionContext.Token,
            "Activité mise à jour !"
        );

        return !reply.IsSuccess ? Result.FromError(reply) : Result.FromSuccess();
    }

    [Command("last")]
    [Description("Recupère la dernière game créée")]
    public async Task<IRemoraResult> GetLastGameAsync()
    {
        IEnumerable<GameDto> games = await _gameService.GetAllAsync();

        Result<IMessage> reply = await _interactionApi.CreateFollowupMessageAsync(
            _interactionContext.ApplicationID,
            _interactionContext.Token,
            new Uri(_options.GeoNRageUrl, $"/games/{games.OrderByDescending(g => g.Date).First().Id}").ToString()
        );

        return !reply.IsSuccess ? Result.FromError(reply) : Result.FromSuccess();
    }
}
