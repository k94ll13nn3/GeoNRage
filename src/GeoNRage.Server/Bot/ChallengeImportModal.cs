using GeoNRage.Server.Services;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Interactivity;
using IRemoraResult = Remora.Results.IResult;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
internal sealed partial class ChallengeImportModal : InteractionGroup
{
    private readonly ChallengeService _challengeService;
    private readonly FeedbackService _feedbackService;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    [Modal("import-challenge")]
    public async Task<IRemoraResult> OnModalSubmitAsync(string challengeGeoguessrId)
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
                Colour: System.Drawing.Color.FromArgb(66, 88, 255),
                Footer: new EmbedFooter("Rageux/20"),
                Fields: fields);

            return await _feedbackService.SendContextualEmbedAsync(embed, options: new() { MessageFlags = MessageFlags.Ephemeral }, ct: CancellationToken);
        }
        catch (InvalidOperationException e)
        {
            return await ReplyEphemeralAsync(e.Message);
        }
    }

    private async Task<IRemoraResult> ReplyEphemeralAsync(string message)
    {
        return await _feedbackService.SendContextualAsync(
            message,
            options: new(MessageFlags: MessageFlags.Ephemeral));
    }
}
