using System;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Components.Games;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;

namespace GeoNRage.App.Pages
{
    public partial class GamePage : IAsyncDisposable
    {
        private HubConnection _hubConnection = null!;

        public GameDto Game { get; set; } = null!;

        [Parameter]
        public int Id { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        public GameChart Chart { get; set; } = null!;

        public bool GameFound { get; set; } = true;

        public bool Loaded { get; set; }

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

            ApiResponse<GameDto> response = await GamesApi.GetAsync(Id);
            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                GameFound = false;
            }
            else
            {
                Loaded = true;
                GameFound = true;
                Game = response.Content;
                await _hubConnection.InvokeAsync("JoinGroup", Id);
                StateHasChanged();
            }
        }

        private async Task HandleReceiveValueAsync(int mapId, int playerId, int round, int score)
        {
            Game[mapId, playerId, round] = score;
            await Chart.UpdateAsync(playerId);
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
