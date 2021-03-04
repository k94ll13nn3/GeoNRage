using System;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class IndexPage
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        public Uri Link { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            GameLightDto[] games = await GamesApi.GetAllLightAsync();
            if (games.Length > 0)
            {
                Link = NavigationManager.ToAbsoluteUri($"/games/{games[0].Id}");
            }

            StateHasChanged();
        }
    }
}
