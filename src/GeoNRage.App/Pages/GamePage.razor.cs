using System;
using System.Collections.Generic;
using System.Linq;
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

        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        public GameChart Chart { get; set; } = null!;

        public bool GameFound { get; set; } = true;

        public bool Loaded { get; set; }

        public bool HubClosed { get; set; }

        public bool HubReconnecting { get; set; }

        public bool HubReconnected { get; set; }

        public IEnumerable<PlayerDto> AvailablePlayers { get; set; } = Enumerable.Empty<PlayerDto>();

        public int SelectedPlayerId { get; set; }

        public async ValueTask DisposeAsync()
        {
            _hubConnection.Closed -= OnHubConnectionClosed;
            _hubConnection.Reconnecting -= OnHubConnectionReconnecting;
            _hubConnection.Reconnected -= OnHubConnectionReconnected;
            await _hubConnection.DisposeAsync();
        }

        public async Task ReloadPageAsync()
        {
            HubClosed = false;
            HubReconnected = false;
            HubReconnecting = false;
            Loaded = false;
            await OnInitializedAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/apphub"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<int, int, int, int>("ReceiveValue", HandleReceiveValueAsync);

            _hubConnection.Closed += OnHubConnectionClosed;
            _hubConnection.Reconnecting += OnHubConnectionReconnecting;
            _hubConnection.Reconnected += OnHubConnectionReconnected;

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
                AvailablePlayers = (await PlayersApi.GetAllAsync()).Where(p => !Game.Players.Any(gp => gp.Id == p.Id));
                SelectedPlayerId = AvailablePlayers.FirstOrDefault()?.Id ?? 0;
                StateHasChanged();
            }
        }

        private Task OnHubConnectionReconnected(string arg)
        {
            HubClosed = false;
            HubReconnected = true;
            HubReconnecting = false;
            StateHasChanged();
            return Task.FromResult(0);
        }

        private Task OnHubConnectionReconnecting(Exception arg)
        {
            HubClosed = false;
            HubReconnected = false;
            HubReconnecting = true;
            StateHasChanged();
            return Task.FromResult(0);
        }

        private Task OnHubConnectionClosed(Exception arg)
        {
            HubClosed = true;
            HubReconnected = false;
            HubReconnecting = false;
            StateHasChanged();
            return Task.FromResult(0);
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

        private async Task AddPlayerAsync()
        {
            if (Game is not null && SelectedPlayerId != 0)
            {
                await GamesApi.AddPlayerAsync(Game.Id, SelectedPlayerId);
                await ReloadPageAsync();
            }
        }
    }
}
