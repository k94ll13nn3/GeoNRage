using GeoNRage.Server.Services;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Interactivity;
using Remora.Rest.Core;
using Remora.Results;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
public partial class ModalReceiver : IModalInteractiveEntity
{
    private readonly IDiscordRestInteractionAPI _interactionApi;
    private readonly InteractionContext _interactionContext;
    private readonly ChallengeService _challengeService;
    private readonly FeedbackService _feedbackService;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    public Task<Result<bool>> IsInterestedAsync(ComponentType? componentType, string customID, CancellationToken ct = default)
    {
        return Task.FromResult(Result<bool>.FromSuccess(customID == "challenge-import"));
    }

    public async Task<Result> HandleInteractionAsync(IUser user, string customID, IReadOnlyList<IPartialMessageComponent> components, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(components);

        var actionRow = (PartialActionRowComponent)components[0];
        Optional<string> textInput = ((PartialTextInputComponent)actionRow.Components.Value[0]).Value;

        try
        {
            int id = await _challengeService.ImportChallengeAsync(new() { GeoGuessrId = textInput.Value, OverrideData = true });
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

            Result<IMessage> reply = await _feedbackService.SendContextualEmbedAsync(embed, options: new() { MessageFlags = MessageFlags.Ephemeral }, ct: ct);

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
            _interactionContext.ApplicationID,
            _interactionContext.Token,
            message,
            flags: MessageFlags.Ephemeral
        );

        return !reply.IsSuccess ? Result.FromError(reply) : Result.FromSuccess();
    }
}
