using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class GamesPage
    {
        public ICollection<GameDto> Games { get; } = new List<GameDto>();

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            GameDto[] games = await GamesApi.GetAllAsync();
            Games.Clear();
            foreach (GameDto game in games)
            {
                Games.Add(game);
            }

            StateHasChanged();
        }
    }
}
