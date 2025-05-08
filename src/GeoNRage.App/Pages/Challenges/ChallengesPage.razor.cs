using System.Security.Claims;
using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Refit;

namespace GeoNRage.App.Pages.Challenges;

[AutoConstructor]
public sealed partial class ChallengesPage
{
    private readonly NavigationManager _navigationManager;
    private readonly IChallengesApi _challengesApi;
    private readonly ModalService _modalService;
    private readonly ToastService _toastService;

    private string _geoGuessrId = null!;
    private bool _displayAll;
    private ClaimsPrincipal _user = null!;
    private PaginatedTable<ChallengeDto> _challengesTable = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    protected override void Dispose(bool disposing)
    {
        _challengesTable?.Dispose();
        base.Dispose(disposing);
    }

    protected override async Task OnInitializedAsync()
    {
        _user = (await AuthenticationState).User;
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        if (e.ChangedKey != nameof(UserSettings.AllMaps))
        {
            return;
        }

        await FilterChallengesAsync(_displayAll);
    }

    private async Task ImportAsync()
    {
        ModalResult result = await _modalService.DisplayOkCancelPopupAsync("Importation", "Valider l'importation du challenge ?");
        if (result.Cancelled)
        {
            return;
        }

        if (string.IsNullOrEmpty(_geoGuessrId))
        {
            _toastService.DisplayToast("Veuillez remplir l'id", TimeSpan.FromSeconds(3), ToastType.Error);
            return;
        }

        await _modalService.DisplayLoaderAsync(async () =>
        {
            try
            {
                await _challengesApi.ImportChallengeAsync(new() { GeoGuessrId = _geoGuessrId, OverrideData = true });
                await FilterChallengesAsync(_displayAll);
                _geoGuessrId = string.Empty;
                _toastService.DisplayToast("Import réussi !", TimeSpan.FromSeconds(3), ToastType.Success);
            }
            catch (ApiException e)
            {
                await _toastService.DisplayErrorToastAsync(e, "challenge-import");
            }
            finally
            {
                StateHasChanged();
            }
        });
    }

    private async Task FilterChallengesAsync(bool displayAll)
    {
        _displayAll = displayAll;
        await _challengesTable.ReloadDataAsync();
    }

    private async Task<PaginationResult<ChallengeDto>> GetAllChallengesAsync(IPaginationQuery paginationQuery)
    {
        string[] playersToHide = [];
        if (!_displayAll && _user.PlayerId() is string playerId)
        {
            playersToHide = [playerId];
        }

        return await _challengesApi.GetAllAsync(paginationQuery, playersToHide);
    }
}
