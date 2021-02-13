using System;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Data.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace GeoNRage.App.Pages
{
    public partial class GamePage : IAsyncDisposable
    {
        private bool _canRender = true;
        private bool _nextRender;
        private HubConnection _hubConnection = null!;

        public Game Game { get; set; } = null!;

        [Parameter]
        public int Id { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/apphub"))
                .Build();

            _hubConnection.On<int, int, int, int>("ReceiveValue", HandleReceiveValue);

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
                UpdatePage();
            }
        }

        protected override bool ShouldRender()
        {
            return _canRender;
        }

        private void HandleReceiveValue(int mapId, int playerId, int round, int score)
        {
            Game[mapId, playerId, round] = score;
            UpdatePage();
        }

        private void Send(int mapId, int playerId, int round, int score)
        {
            int clampedValue = Math.Clamp(score, 0, 5000);
            _hubConnection.InvokeAsync("UpdateValue", Id, mapId, playerId, round, clampedValue);
            HandleReceiveValue(mapId, playerId, round, clampedValue);
        }

        private void InputFocused(bool focused)
        {
            _canRender = !focused;
            if (!focused && _nextRender)
            {
                UpdatePage();
            }
        }

        private void UpdatePage()
        {
            if (_canRender)
            {
                StateHasChanged();
                _nextRender = false;
            }
            else
            {
                _nextRender = true;
            }
        }
    }
}
