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

        protected override async Task OnInitializedAsync()
        {
            GameDto[] games = await GamesApi.GetAllAsync();
            if (games.Length > 0)
            {
                NavigationManager.NavigateTo($"/games/{games[0].Id}");
            }

            StateHasChanged();
        }
    }
}
