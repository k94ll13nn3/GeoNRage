using GeoNRage.Server.Services;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Interactivity;
using Remora.Results;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
public partial class ChallengeImportModal : InteractionGroup
{
    private readonly IDiscordRestInteractionAPI _interactionApi;
    private readonly IInteractionContext _interactionContext;
    private readonly ChallengeService _challengeService;
    private readonly FeedbackService _feedbackService;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    [Modal("import-challenge")]
    public async Task<Result> OnModalSubmitAsync(string challengeGeoguessrId)
    {
        try
        {
            int id = await _challengeService.ImportChallengeAsync(new() { GeoGuessrId = challengeGeoguessrId, OverrideData = true });
            ChallengeDetailDto? challenge = await _challengeService.GetAsync(id);

            if (challenge is null)
            {
                return await ReplyEphemeralAsync("Import réussi.");
            }

            var fields = new List<EmbedField>
            {
                new ("Joueurs", string.Join(Environment.NewLine, challenge.PlayerScores.Select(p => p.PlayerName).Distinct()), true),
            };

            var embed = new Embed(
                Title: challenge.MapName,
                Type: EmbedType.Rich,
                Url: new Uri(_options.GeoNRageUrl, $"/challenges/{id}").ToString(),
                Colour: System.Drawing.Color.FromArgb(0x37, 0x5a, 0x7f),
                Footer: new EmbedFooter("Rageux/20"),
                Fields: fields);

            Result<IMessage> reply = await _feedbackService.SendContextualEmbedAsync(embed, options: new() { MessageFlags = MessageFlags.Ephemeral }, ct: CancellationToken);

            return !reply.IsSuccess
                ? Result.FromError(reply)
                : Result.FromSuccess();
        }
        catch (InvalidOperationException e)
        {
            return await ReplyEphemeralAsync(e.Message);
        }
    }

    private async Task<Result> ReplyEphemeralAsync(string message)
    {
        Result<IMessage> reply = await _interactionApi.CreateFollowupMessageAsync(
            _interactionContext.Interaction.ApplicationID,
            _interactionContext.Interaction.Token,
            message,
            flags: MessageFlags.Ephemeral
        );

        return !reply.IsSuccess ? Result.FromError(reply) : Result.FromSuccess();
    }
}
