using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages.Challenges;

public partial class ChallengePage
{
    [Inject]
    public IChallengesApi ChallengesApi { get; set; } = null!;

    [Parameter]
    public int Id { get; set; }

    [Inject]
    public PopupService PopupService { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public bool ChallengeFound { get; set; } = true;

    public bool Loaded { get; set; }

    public ChallengeDetailDto Challenge { get; set; } = null!;

    public Table<ChallengePlayerScoreDto> ChallengeTable { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ApiResponse<ChallengeDetailDto> response = await ChallengesApi.GetAsync(Id);
        if (!response.IsSuccessStatusCode || response.Content is null)
        {
            ChallengeFound = false;
        }
        else
        {
            Loaded = true;
            ChallengeFound = true;
            Challenge = response.Content;
            StateHasChanged();
        }
    }

    private void Refresh()
    {
        PopupService.DisplayOkCancelPopup("Mise à jour", "Mettre à jour les scores du challenge ?", async () => await RefreshAsync());
    }

    private async Task RefreshAsync()
    {
        try
        {
            PopupService.DisplayLoader("Mise à jour");
            await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = Challenge.GeoGuessrId, OverrideData = true });
            ApiResponse<ChallengeDetailDto> response = await ChallengesApi.GetAsync(Challenge.Id);
            Challenge = response.Content!;
            ChallengeTable?.SetItems(Challenge.PlayerScores.OrderByDescending(p => p.PlayerGuesses.Sum(g => g.Score)));
        }
        catch (ApiException e)
        {
            ToastService.DisplayToast(e.Content ?? "Echec de l'opération", null, ToastType.Error, "challenge-refresh", true);
        }
        finally
        {
            PopupService.HidePopup();
            StateHasChanged();
        }
    }
}
