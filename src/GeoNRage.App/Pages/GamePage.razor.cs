using System;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using GeoNRage.Data.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace GeoNRage.App.Pages
{
    public partial class GamePage : IAsyncDisposable
    {
        private HubConnection _hubConnection = null!;

        public Game Game { get; set; } = null!;

        [Parameter]
        public int Id { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        public GameChart Chart { get; set; } = null!;

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/apphub"))
                .Build();

            _hubConnection.On<int, int, int, int>("ReceiveValue", HandleReceiveValueAsync);

            await _hubConnection.StartAsync();

            Game? game = await GamesApi.GetAsync(Id);
            if (game is null)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                Game = game;
                await _hubConnection.InvokeAsync("JoinGroup", Id);
                StateHasChanged();
            }
        }

        private async Task HandleReceiveValueAsync(int mapId, int playerId, int round, int score)
        {
            Game[mapId, playerId, round] = score;
            await Chart.UpdateAsync(mapId, playerId, round, score);
            StateHasChanged();
        }

        private async Task SendAsync(int mapId, int playerId, int round, int score)
        {
            int clampedValue = Math.Clamp(score, 0, 5000);
            await _hubConnection.InvokeAsync("UpdateValue", Id, mapId, playerId, round, clampedValue);
            await HandleReceiveValueAsync(mapId, playerId, round, clampedValue);
        }
    }
}
