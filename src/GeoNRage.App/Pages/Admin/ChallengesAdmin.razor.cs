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
    public PopupService PopupService { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public IEnumerable<ChallengeAdminViewDto> Challenges { get; set; } = null!;

    public string? Error { get; set; }

    public Table<ChallengeAdminViewDto> ChallengesTable { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Challenges = await ChallengesApi.GetAllAsAdminViewAsync();
    }

    private void ImportChallenge(ChallengeAdminViewDto challenge)
    {
        PopupService.DisplayOkCancelPopup("Restoration", "Valider la restoration du challenge ?", async () => await ImportChallengeAsync(challenge));
    }

    private void DeleteChallenge(int challengeId)
    {
        PopupService.DisplayOkCancelPopup("Suppression", $"Valider la suppression du challenge {challengeId} ?", () => OnConfirmDeleteAsync(challengeId));
    }

    private async void OnConfirmDeleteAsync(int challengeId)
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

    private async Task ImportChallengeAsync(ChallengeAdminViewDto challenge)
    {
        _ = challenge ?? throw new ArgumentNullException(nameof(challenge));

        PopupService.DisplayLoader("Restoration");

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
            Error = $"Error: {(await e.GetContentAsAsync<ApiError>())?.Message}";
        }
        finally
        {
            await OnInitializedAsync();
            ChallengesTable.SetItems(Challenges);
            PopupService.HidePopup();
            StateHasChanged();
        }
    }
}
