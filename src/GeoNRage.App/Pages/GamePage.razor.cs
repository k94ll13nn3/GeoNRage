using System;
using System.Threading.Tasks;
using GeoNRage.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace GeoNRage.App.Pages
{
    public partial class GamePage : IAsyncDisposable
    {
        private HubConnection _hubConnection = null!;

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        public Game? Game { get; set; }

        [Parameter]
        public int Id { get; set; }

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

            _hubConnection.On<Game>("ReceiveGame", HandleReceiveGameAsync);

            _hubConnection.On<string, int>("ReceiveRow", (name, value) =>
            {
                if (Game is not null)
                {
                    Game.Values[name] = value;
                }

                StateHasChanged();
            });

            await _hubConnection.StartAsync();
            await _hubConnection.SendAsync("LoadGame", Id);
        }

        private async Task HandleReceiveGameAsync(Game game)
        {
            if (game is null)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                Game = game;
                StateHasChanged();
                await _hubConnection.SendAsync("JoinGroup", Id);
            }
        }

        private void Send(string name, ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString()?.Replace(",", ".", StringComparison.InvariantCulture) ?? "0", out int value))
            {
                if (Game is not null)
                {
                    Game.Values[name] = value;
                }
                _hubConnection.SendAsync("SendMessage", Id, name, value);
            }
        }
    }
}
