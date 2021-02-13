using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Data.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace GeoNRage.App.Pages
{
    public partial class GamesPage : IAsyncDisposable
    {
        private HubConnection _hubConnection = null!;

        public ICollection<Game> Games { get; } = new List<Game>();

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/apphub"))
                .Build();

            _hubConnection.On<IEnumerable<Game>>("ReceiveGames", games =>
            {
                Games.Clear();
                foreach (Game game in games)
                {
                    Games.Add(game);
                }

                StateHasChanged();
            });

            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("LoadGames");
        }
    }
}
