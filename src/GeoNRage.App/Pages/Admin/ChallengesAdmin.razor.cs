using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages.Admin;

public partial class ChallengesAdmin
{
    [Inject]
    public IGamesApi GamesApi { get; set; } = null!;

    [Inject]
    public IChallengesApi ChallengesApi { get; set; } = null!;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public IEnumerable<ChallengeAdminViewDto> Challenges { get; set; } = null!;

    public string? Error { get; set; }

    public Table<ChallengeAdminViewDto> ChallengesTable { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Challenges = await ChallengesApi.GetAllAsAdminViewAsync();
    }

    private async Task ImportChallengeAsync(ChallengeAdminViewDto challenge)
    {
        ModalResult result = await ModalService.DisplayOkCancelPopupAsync("Restoration", "Valider la restoration du challenge ?");
        if (!result.Cancelled)
        {
            await ModalService.DisplayLoaderAsync(async () =>
            {
                try
                {
                    Error = null;
                    if (challenge.GameId == -1)
                    {
                        await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = challenge.GeoGuessrId, OverrideData = true });
                    }
                    else
                    {
                        await GamesApi.UpdateChallengesAsync(challenge.GameId);
                    }
                }
                catch (ApiException e)
                {
                    Error = $"Error: {(await e.GetContentAsAsync<ProblemDetails>())?.Detail}";
                }
                finally
                {
                    await OnInitializedAsync();
                    ChallengesTable.SetItems(Challenges);
                    StateHasChanged();
                }
            });
        }
    }

    private async Task DeleteChallengeAsync(int challengeId)
    {
        ModalResult result = await ModalService.DisplayOkCancelPopupAsync("Suppression", $"Valider la suppression du challenge {challengeId} ?");
        if (!result.Cancelled)
        {
            try
            {
                await ChallengesApi.DeleteAsync(challengeId);
                await OnInitializedAsync();
                ChallengesTable.SetItems(Challenges);
                StateHasChanged();
            }
            catch (ApiException e)
            {
                await ToastService.DisplayErrorToastAsync(e, "challenge-delete");
            }
        }
    }
}
