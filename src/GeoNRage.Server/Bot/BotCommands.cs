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
        Result<IApplication> bot = await _oauth2API.GetCurrentBotApplicationInformationAsync();
        if (!bot.IsSuccess || bot.Entity.Icon is null)
        {
            return await ReplyAsync("Erreur inconnue");
        }

        IEnumerable<GameDto> games = await _gameService.GetAllAsync();
        GameDetailDto? game = await _gameService.GetAsync(games.OrderByDescending(g => g.Date).First().Id);
        if (game is null)
        {
            return await ReplyAsync("Erreur inconnue");
        }

        var fields = new List<EmbedField>()
        {
            new ("Cartes", string.Join(", ", game.Challenges.Select(c => c.MapName))),
            new ("Joueurs", string.Join(", ", game.Players.Select(c => c.Name))),
        };

        var embed = new Embed(
            Title: game.Name,
            Type: EmbedType.Rich,
            Url: new Uri(_options.GeoNRageUrl, $"/games/{game.Id}").ToString(),
            Colour: System.Drawing.Color.FromArgb(0x37, 0x5a, 0x7f),
            Footer: new EmbedFooter("Rageux/20"),
            Fields: fields);

        Result<IMessage> reply = await _feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken);

        return !reply.IsSuccess
            ? Result.FromError(reply)
            : Result.FromSuccess();
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

        PlayerFullDto? playerFull = await _playerService.GetFullAsync(player.Id, false);
        if (playerFull is null)
        {
            return await ReplyAsync("Erreur inconnue");
        }

        var fields = new List<EmbedField>
        {
            new("Nombre de carte complétées", $"{playerFull.ChallengesDone.Count()}"),
            new("Nombre de 5000", $"{playerFull.Statistics.NumberOf5000}", true),
            new("Meilleure partie", playerFull.Statistics.BestGameSum.FormatNullWithDash(), true),
            new("Nombre de 25k", $"{playerFull.Statistics.NumberOf25000}", true),
        };

        Result<IApplication> bot = await _oauth2API.GetCurrentBotApplicationInformationAsync();
        if (!bot.IsSuccess || bot.Entity.Icon is null)
        {
            return await ReplyAsync("Erreur inconnue");
        }
        string iconUrl = playerFull.IconUrl.ToString();
        if (iconUrl == Constants.BaseAvatarUrl.ToString())
        {
            iconUrl = new Uri(_options.GeoNRageUrl, iconUrl).ToString();
        }

        var embed = new Embed(
            Title: player.Name,
            Type: EmbedType.Rich,
            Url: new Uri(_options.GeoNRageUrl, $"/players/{player.Id}").ToString(),
            Colour: System.Drawing.Color.FromArgb(0x37, 0x5a, 0x7f),
            Thumbnail: new EmbedThumbnail(iconUrl),
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
