using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace GeoNRage.App.Pages
{
    public partial class GamePage : IAsyncDisposable
    {
        private bool _canRender = true;
        private bool _nextRender;
        private HubConnection _hubConnection = null!;

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        public Game? Game { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public Dictionary<string, (int total, int position)> Totals { get; set; } = new();

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

            _hubConnection.On<string, int>("ReceiveValue", HandleReceiveValue);

            await _hubConnection.StartAsync();
            await _hubConnection.SendAsync("LoadGame", Id);
        }

        private void ComputeTotals()
        {
            foreach (string map in Game.Maps)
            {
                var scores = new List<(string key, int score)>();
                foreach (string column in Game.Columns)
                {
                    scores.Add(($"{map}_{column}", Game.Values.Where(x => x.Key.StartsWith($"{map}_{column}_")).Sum(x => x.Score)));
                }

                foreach ((string key, int score, int index) item in scores.OrderByDescending(x => x.score).Select((item, index) => (item.key, item.score, index: index + 1)))
                {
                    Totals[item.key] = (item.score, item.index);
                }
            }

            var scores2 = new List<(string key, int score)>();
            foreach (string column in Game.Columns)
            {
                scores2.Add(($"{column}", Game.Values.Where(x => x.Key.Contains($"_{column}_")).Sum(x => x.Score)));
            }

            foreach ((string key, int score, int index) item in scores2.OrderByDescending(x => x.score).Select((item, index) => (item.key, item.score, index: index + 1)))
            {
                Totals[item.key] = (item.score, item.index);
            }
        }

        private void HandleReceiveValue(string key, int score)
        {
            Game[key] = score;
            ComputeTotals();
            UpdatePage();
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
                await _hubConnection.SendAsync("JoinGroup", Id);

                ComputeTotals();
                UpdatePage();
            }
        }

        private void Send(string key, int score)
        {
            int clampedValue = Math.Clamp(score, 0, 5000);
            Game[key] = clampedValue;

            _hubConnection.SendAsync("UpdateValue", Id, key, clampedValue);
            ComputeTotals();
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
