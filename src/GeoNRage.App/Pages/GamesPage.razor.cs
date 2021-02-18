using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class GamesPage
    {
        public ICollection<GameLightDto> Games { get; } = new List<GameLightDto>();

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            GameLightDto[] games = await GamesApi.GetAllLightAsync();
            Games.Clear();
            foreach (GameLightDto game in games)
            {
                Games.Add(game);
            }

            StateHasChanged();
        }
    }
}
