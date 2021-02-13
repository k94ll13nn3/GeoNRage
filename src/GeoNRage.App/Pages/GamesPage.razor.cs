using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GeoNRage.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class GamesPage
    {
        public ICollection<Game> Games { get; } = new List<Game>();

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public HttpClient HttpClient { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            Game[]? games = await HttpClient.GetFromJsonAsync<Game[]>("games");
            if (games is not null)
            {
                Games.Clear();
                foreach (Game game in games)
                {
                    Games.Add(game);
                }
            }

            StateHasChanged();
        }
    }
}
