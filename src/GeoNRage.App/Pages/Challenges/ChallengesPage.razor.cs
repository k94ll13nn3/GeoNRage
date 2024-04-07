using System.Security.Claims;
using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Refit;

namespace GeoNRage.App.Pages.Challenges;

public partial class ChallengesPage
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IChallengesApi ChallengesApi { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public IEnumerable<ChallengeDto> Challenges { get; set; } = null!;

    public string GeoGuessrId { get; set; } = null!;

    public Table<ChallengeDto> ChallengesTable { get; set; } = null!;

    public bool DisplayAll { get; set; }

    public ClaimsPrincipal User { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        User = (await AuthenticationState).User;
        await FilterChallengesAsync(DisplayAll);
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        if (e.ChangedKey != nameof(UserSettings.AllMaps))
        {
            return;
        }

        await FilterChallengesAsync(DisplayAll);
    }

    private static IEnumerable<ChallengeDto> Sort(IEnumerable<ChallengeDto> challenges, string column, bool ascending)
    {
        return column switch
        {
            nameof(ChallengeDto.MapName) => ascending ? challenges.OrderBy(c => c.MapName) : challenges.OrderByDescending(c => c.MapName),
            nameof(ChallengeDto.MaxScore) => ascending ? challenges.OrderBy(c => c.MaxScore) : challenges.OrderByDescending(c => c.MaxScore),
            nameof(ChallengeDto.PlayerScore) => ascending ? challenges.OrderBy(c => c.PlayerScore) : challenges.OrderByDescending(c => c.PlayerScore),
            _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
        };
    }

    private async Task ImportAsync()
    {
        ModalResult result = await ModalService.DisplayOkCancelPopupAsync("Importation", "Valider l'importation du challenge ?");
        if (!result.Cancelled)
        {
            if (string.IsNullOrEmpty(GeoGuessrId))
            {
                ToastService.DisplayToast("Veuillez remplir l'id", TimeSpan.FromSeconds(3), ToastType.Error);
                return;
            }

            await ModalService.DisplayLoaderAsync(async () =>
            {
                try
                {
                    await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = GeoGuessrId, OverrideData = true });
                    await FilterChallengesAsync(DisplayAll);
                    GeoGuessrId = string.Empty;
                    ToastService.DisplayToast("Import réussi !", TimeSpan.FromSeconds(3), ToastType.Success);
                }
                catch (ValidationApiException e)
                {
                    string error = string.Join(",", e.Content?.Errors.Select(x => string.Join(",", x.Value)) ?? []);
                    ToastService.DisplayToast(error, null, ToastType.Error);
                }
                catch (ApiException e)
                {
                    await ToastService.DisplayErrorToastAsync(e, "challenge-import");
                }
                finally
                {
                    StateHasChanged();
                }
            });
        }
    }

    private async Task FilterChallengesAsync(bool displayAll)
    {
        DisplayAll = displayAll;
        string[] playersToHide = [];
        if (!DisplayAll && User.PlayerId() is string playerId)
        {
            playersToHide = [playerId];
        }

        Challenges = await ChallengesApi.GetAllAsync(true, playersToHide);
        ChallengesTable?.SetItems(Challenges);
    }
}
