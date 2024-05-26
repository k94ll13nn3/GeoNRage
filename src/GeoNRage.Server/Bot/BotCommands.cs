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
    private readonly IInteractionContext _interactionContext;
    private readonly GameService _gameService;
    private readonly PlayerService _playerService;
    private readonly MapService _mapService;
    private readonly FeedbackService _feedbackService;
    private readonly IDiscordRestOAuth2API _oauth2API;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    [Command("last")]
    [Description("Recupère la dernière partie créée")]
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
            Colour: System.Drawing.Color.FromArgb(66, 88, 255),
            Footer: new EmbedFooter("Rageux/20"),
            Fields: fields);

        Result<IMessage> reply = await _feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken);

        return !reply.IsSuccess
            ? Result.FromError(reply)
            : Result.FromSuccess();
    }

    [Command("player")]
    [Description("Affiche les stats du joueur")]
    public async Task<IRemoraResult> GetPlayerStatisticsAsync(
        [AutocompleteProvider(nameof(PlayerNameAutocompleteProvider))]
        [Description("L'id du joueur, peut-être récupéré via autocompletion")]
        string playerId,
        [Description("Valeur indiquant si toutes les cartes doivent être prise ou seulement les principales")]
        bool allMaps = false)
    {
        PlayerFullDto? playerFull = await _playerService.GetFullAsync(playerId, allMaps);
        if (playerFull is null)
        {
            return await ReplyAsync("Joueur inconnu");
        }

        var fields = new List<EmbedField>
        {
            new(LabelStore.Get(() => playerFull.Statistics.ChallengesCompleted), $"{playerFull.Statistics.ChallengesCompleted}"),
            new(LabelStore.Get(() => playerFull.Statistics.NumberOf5000), $"{playerFull.Statistics.NumberOf5000}", true),
            new(LabelStore.Get(() => playerFull.Statistics.BestGameSum), playerFull.Statistics.BestGameSum.FormatNullWithDash(), true),
            new(LabelStore.Get(() => playerFull.Statistics.NumberOf25000), $"{playerFull.Statistics.NumberOf25000}", true),
            new(LabelStore.Get(() => playerFull.Statistics.NumberOfGamesPlayed), $"{playerFull.Statistics.NumberOfGamesPlayed}"),
            new(LabelStore.Get(() => playerFull.Statistics.NumberOfFirstPlaceInGame), $"{playerFull.Statistics.NumberOfFirstPlaceInGame}", true),
            new(LabelStore.Get(() => playerFull.Statistics.NumberOfSecondPlaceInGame), $"{playerFull.Statistics.NumberOfSecondPlaceInGame}", true),
            new(LabelStore.Get(() => playerFull.Statistics.NumberOfThirdPlaceInGame), $"{playerFull.Statistics.NumberOfThirdPlaceInGame}", true),
        };

        string iconUrl = playerFull.IconUrl.ToString();
        if (iconUrl == Constants.BaseAvatarUrl.ToString())
        {
            iconUrl = new Uri(_options.GeoNRageUrl, iconUrl).ToString();
        }

        var embed = new Embed(
            Title: playerFull.Name,
            Type: EmbedType.Rich,
            Description: playerFull.Title,
            Url: new Uri(_options.GeoNRageUrl, $"/players/{playerFull.Id}").ToString(),
            Colour: System.Drawing.Color.FromArgb(66, 88, 255),
            Footer: new EmbedFooter("Rageux/20"),
            Thumbnail: new EmbedThumbnail(iconUrl),
            Fields: fields);

        Result<IMessage> reply = await _feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken);

        return !reply.IsSuccess
            ? Result.FromError(reply)
            : Result.FromSuccess();
    }

    [Command("map")]
    [Description("Affiche les stats de la carte")]
    public async Task<IRemoraResult> GetMapStatisticsAsync(
        [AutocompleteProvider(nameof(MapNameAutocompleteProvider))]
        [Description("L'id de la carte, peut-être récupéré via autocompletion")]
        string mapId,
        [Description("Valeur indiquant si toutes les cartes doivent être prise ou seulement les principales")]
        bool allMaps = false)
    {
        MapStatisticsDto? statistics = await _mapService.GetMapStatisticsAsync(mapId, allMaps);
        if (statistics is null)
        {
            return await ReplyAsync("Carte inconnue");
        }

        List<string> rankings = statistics
            .Scores
            .OrderByDescending(s => s.Sum)
            .ThenBy(s => s.Time)
            .Select(s => $"{s.PlayerName} : {s.Sum} ({s.Time.ToTimeString()})")
            .ToList();

        if (rankings.Count == 0 && !allMaps)
        {
            return await ReplyAsync($"Veuillez utiliser le paramètre '{nameof(allMaps)}' pour cette carte");
        }

        if (rankings.Count == 0)
        {
            return await ReplyAsync("Erreur inconnue");
        }

        var fields = new List<EmbedField>
        {
            new ("Classement (1 à 5)", string.Join(Environment.NewLine, rankings.Take(5)), true),
        };

        if (rankings.Count > 5)
        {
            fields.Add(new("Classement (6 à 10)", string.Join(Environment.NewLine, rankings.Take(5..10)), true));
        }

        var embed = new Embed(
            Title: statistics.Name,
            Type: EmbedType.Rich,
            Colour: System.Drawing.Color.FromArgb(66, 88, 255),
            Footer: new EmbedFooter("Rageux/20"),
            Fields: fields);

        Result<IMessage> reply = await _feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken);

        return !reply.IsSuccess
            ? Result.FromError(reply)
            : Result.FromSuccess();
    }

    [Command("import-challenge")]
    [Description("Importer un challenge")]
    [SuppressInteractionResponse(true)]
    public async Task<Result> ShowImportModalAsync()
    {
        var response = new InteractionResponse
        (
            InteractionCallbackType.Modal,
            new
            (
                new InteractionModalCallbackData
                (
                    Remora.Discord.Interactivity.CustomIDHelpers.CreateModalID("import-challenge"),
                    "Importer un challenge",
                    [
                        new ActionRowComponent
                        (
                           [
                                new TextInputComponent
                                (
                                    "challengeGeoguessrId",
                                    TextInputStyle.Short,
                                    "Id du challenge",
                                    16,
                                    17,
                                    true,
                                    default,
                                    "Id GeoGuessr"
                                )
                            ]
                        )
                    ]
                )
            )
        );

        return await _interactionApi.CreateInteractionResponseAsync
        (
            _interactionContext.Interaction.ID,
            _interactionContext.Interaction.Token,
            response,
            ct: CancellationToken
        );
    }

    private async Task<Result> ReplyAsync(string message)
    {
        Result<IMessage> reply = await _interactionApi.CreateFollowupMessageAsync(
            _interactionContext.Interaction.ApplicationID,
            _interactionContext.Interaction.Token,
            message
        );

        return !reply.IsSuccess ? Result.FromError(reply) : Result.FromSuccess();
    }
}
