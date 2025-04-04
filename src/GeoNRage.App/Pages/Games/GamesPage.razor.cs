using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Games;

public partial class GamesPage
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IGamesApi GamesApi { get; set; } = null!;

    public IEnumerable<GameDto> Games { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Games = await GamesApi.GetAllAsync();
    }
}
