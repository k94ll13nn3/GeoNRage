using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace GeoNRage.App.Pages
{
    public partial class Index : IAsyncDisposable
    {
        private HubConnection _hubConnection = null!;

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        public IEnumerable<GameBase> Games { get; set; } = Array.Empty<GameBase>();

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5011/apphub")
                .Build();

            _hubConnection.On<IEnumerable<GameBase>>("ReceiveGames", games =>
            {
                Games = games;
                StateHasChanged();
            });

            await _hubConnection.StartAsync();
            await _hubConnection.SendAsync("LoadGames");
        }
    }
}
