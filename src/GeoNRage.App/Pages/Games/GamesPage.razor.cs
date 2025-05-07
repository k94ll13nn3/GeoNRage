using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Games;

[AutoConstructor]
public sealed partial class GamesPage
{
    private readonly NavigationManager _navigationManager;
    private readonly IGamesApi _gamesApi;
}
