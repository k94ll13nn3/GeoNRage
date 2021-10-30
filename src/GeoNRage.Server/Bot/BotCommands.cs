using System.ComponentModel;
using GeoNRage.Server.Services;
using Microsoft.Extensions.Options;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;
using IRemoraResult = Remora.Results.IResult;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
public partial class BotCommands : CommandGroup
{
    private readonly IDiscordRestInteractionAPI _interactionApi;
    private readonly InteractionContext _interactionContext;
    private readonly GameService _gameService;
    private readonly PlayerService _playerService;
    private readonly FeedbackService _feedbackService;
    private readonly IDiscordRestOAuth2API _oauth2API;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    [Command("last")]
    [Description("Recupère la dernière game créée")]
    public async Task<IRemoraResult> GetLastGameAsync()
    {
        IEnumerable<GameDto> games = await _gameService.GetAllAsync();

        return await ReplyAsync(new Uri(_options.GeoNRageUrl, $"/games/{games.OrderByDescending(g => g.Date).First().Id}").ToString());
    }

    [Command("player")]
    [Description("Affiche les stats du joueur")]
    public async Task<IRemoraResult> GetPlayerStatisticsAsync([AutocompleteProvider(nameof(PlayerNameAutocompleteProvider))] string playerName)
    {
        IEnumerable<PlayerDto> players = await _playerService.GetAllAsync();

        if (players.FirstOrDefault(p => p.Name.RemoveDiacritics().Contains(playerName.RemoveDiacritics(), StringComparison.OrdinalIgnoreCase)) is not PlayerDto player)
        {
            return await ReplyAsync("Joueur inconnu");
        }

        PlayerFullDto? stats = await _playerService.GetFullAsync(player.Id, false);
        if (stats is null)
        {
            return await ReplyAsync("Erreur inconnue");
        }

        var fields = new List<EmbedField>
        {
            new("Nombre de carte complétées", $"{stats.ChallengesDone.Count()}"),
            new("Nombre de 5000", $"{stats.Statistics.NumberOf5000}", true),
            new("Meilleure partie", stats.Statistics.BestGameSum.FormatNullWithDash(), true),
            new("Nombre de 25k", $"{stats.Statistics.NumberOf25000}", true),
        };

        Result<IApplication> bot = await _oauth2API.GetCurrentBotApplicationInformationAsync();
        if (!bot.IsSuccess || bot.Entity.Icon is null)
        {
            return await ReplyAsync("Erreur inconnue");
        }

        string iconUrl = $"https://cdn.discordapp.com/app-icons/{bot.Entity.ID}/{bot.Entity.Icon.Value}.png";

        var embed = new Embed(
            Title: player.Name,
            Type: EmbedType.Rich,
            Url: new Uri(_options.GeoNRageUrl, $"/players/{player.Id}").ToString(),
            Colour: System.Drawing.Color.FromArgb(0x37, 0x5a, 0x7f),
            Thumbnail: new EmbedThumbnail(iconUrl),
            Timestamp: DateTimeOffset.Now,
            Footer: new EmbedFooter("Rageux/20"),
            Fields: fields);

        Result<IMessage> reply = await _feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken);

        return !reply.IsSuccess
            ? Result.FromError(reply)
            : Result.FromSuccess();
    }

    private async Task<Result> ReplyAsync(string message)
    {
        Result<IMessage> reply = await _interactionApi.CreateFollowupMessageAsync(
            _interactionContext.ApplicationID,
            _interactionContext.Token,
            message
        );

        return !reply.IsSuccess ? Result.FromError(reply) : Result.FromSuccess();
    }
}
