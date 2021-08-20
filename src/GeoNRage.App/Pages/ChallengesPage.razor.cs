using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages;

public partial class ChallengesPage
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IChallengesApi ChallengesApi { get; set; } = null!;

    [Inject]
    public IAuthApi AuthApi { get; set; } = null!;

    [Inject]
    public PopupService PopupService { get; set; } = null!;

    public IEnumerable<ChallengeDto> Challenges { get; set; } = null!;

    public string GeoGuessrId { get; set; } = null!;

    public Table<ChallengeDto> ChallengesTable { get; set; } = null!;

    public string? Error { get; set; }

    public bool ChallengeImported { get; set; }

    public bool DisplayAll { get; set; }

    public UserDto User { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        User = await AuthApi.CurrentUserInfo();
        await FilterChallengesAsync(DisplayAll);
    }

    internal override async void OnMapStatusChanged(object? sender, EventArgs e)
    {
        await FilterChallengesAsync(DisplayAll);
    }

    private static IEnumerable<ChallengeDto> Sort(IEnumerable<ChallengeDto> challenges, string column, bool ascending)
    {
        return column switch
        {
            nameof(ChallengeDto.MapName) => ascending ? challenges.OrderBy(c => c.MapName) : challenges.OrderByDescending(c => c.MapName),
            nameof(ChallengeDto.MaxScore) => ascending ? challenges.OrderBy(c => c.MaxScore) : challenges.OrderByDescending(c => c.MaxScore),
            _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
        };
    }

    private void Import()
    {
        PopupService.DisplayOkCancelPopup("Importation", "Valider l'importation du challenge ?", async () => await ImportAsync(), true);
    }

    private async Task FilterChallengesAsync(bool displayAll)
    {
        DisplayAll = displayAll;
        string[] playersToHide = Array.Empty<string>();
        if (!DisplayAll && User.PlayerId is not null)
        {
            playersToHide = new[] { User.PlayerId };
        }

        Challenges = await ChallengesApi.GetAllAsync(true, playersToHide);
        ChallengesTable?.SetItems(Challenges);
    }

    private async Task ImportAsync()
    {
        try
        {
            Error = null;
            ChallengeImported = false;
            await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = GeoGuessrId, OverrideData = true });
            await FilterChallengesAsync(DisplayAll);
            GeoGuessrId = string.Empty;
            ChallengeImported = true;
        }
        catch (ValidationApiException e)
        {
            Error = string.Join(",", e.Content?.Errors.Select(x => string.Join(",", x.Value)) ?? Array.Empty<string>());
        }
        catch (ApiException e)
        {
            Error = e.Content;
        }
        finally
        {
            PopupService.HidePopup();
            StateHasChanged();
        }
    }
}
