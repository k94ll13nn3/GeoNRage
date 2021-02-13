using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GeoNRage.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class Index
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public HttpClient HttpClient { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            Game[]? games = await HttpClient.GetFromJsonAsync<Game[]>("games");
            if (games?.Length > 0)
            {
                NavigationManager.NavigateTo($"/games/{games[0].Id}");
            }

            StateHasChanged();
        }
    }
}
