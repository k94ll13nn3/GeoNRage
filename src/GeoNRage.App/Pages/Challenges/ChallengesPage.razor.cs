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

    private IEnumerable<ChallengeDto> _challenges = null!;
    private string _geoGuessrId = null!;
    private Table<ChallengeDto> _challengesTable = null!;
    private bool _displayAll;
    private ClaimsPrincipal _user = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        _user = (await AuthenticationState).User;
        await FilterChallengesAsync(_displayAll);
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        if (e.ChangedKey != nameof(UserSettings.AllMaps))
        {
            return;
        }

        await FilterChallengesAsync(_displayAll);
    }

    protected override void Dispose(bool disposing)
    {
        _challengesTable?.Dispose();
        base.Dispose(disposing);
    }

    private static IEnumerable<ChallengeDto> Sort(IEnumerable<ChallengeDto> challenges, string column, bool ascending)
    {
        return column switch
        {
            nameof(ChallengeDto.MapName) => ascending ? challenges.OrderBy(c => c.MapName) : challenges.OrderByDescending(c => c.MapName),
            nameof(ChallengeDto.MaxScore) => ascending ? challenges.OrderBy(c => c.MaxScore) : challenges.OrderByDescending(c => c.MaxScore),
            nameof(ChallengeDto.PlayerScore) => ascending ? challenges.OrderBy(c => c.PlayerScore) : challenges.OrderByDescending(c => c.PlayerScore),
            nameof(ChallengeDto.CreatedAt) => ascending ? challenges.OrderBy(c => c.CreatedAt) : challenges.OrderByDescending(c => c.CreatedAt),
            _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
        };
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
        string[] playersToHide = [];
        if (!_displayAll && _user.PlayerId() is string playerId)
        {
            playersToHide = [playerId];
        }

        _challenges = await _challengesApi.GetAllAsync(true, playersToHide);
        _challengesTable?.SetItems(_challenges);
    }
}
