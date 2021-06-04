using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class GamesPage
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        public IEnumerable<GameLightDto> Games { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            Games = await GamesApi.GetAllLightAsync();
        }
    }
}
