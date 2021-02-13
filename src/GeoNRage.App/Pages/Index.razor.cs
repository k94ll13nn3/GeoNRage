using System.Threading.Tasks;
using GeoNRage.App.Clients;
using GeoNRage.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class Index
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public GamesHttpClient HttpClient { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            Game[]? games = await HttpClient.GetAllAsync();
            if (games?.Length > 0)
            {
                NavigationManager.NavigateTo($"/games/{games[0].Id}");
            }

            StateHasChanged();
        }
    }
}
