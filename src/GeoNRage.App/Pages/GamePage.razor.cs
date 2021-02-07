using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ChartJs.Blazor;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Util;
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

        public LineConfig PlotConfig { get; set; }

        public Chart Chart { get; set; }

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
            await _hubConnection.InvokeAsync("LoadGame", Id);
        }

        private void ComputeTotals()
        {
            foreach (string map in Game.Maps)
            {
                var scores = new List<(string key, int score)>();
                foreach (string player in Game.Players)
                {
                    scores.Add(($"{map}_{player}", Game.Values.Where(x => x.GetMap() == map && x.GetPlayer() == player).Sum(x => x.Score)));
                }

                foreach ((string key, int score, int index) item in scores.OrderByDescending(x => x.score).Select((item, index) => (item.key, item.score, index: index + 1)))
                {
                    Totals[item.key] = (item.score, item.index);
                }
            }

            var scores2 = new List<(string key, int score)>();
            foreach (string player in Game.Players)
            {
                scores2.Add(($"{player}", Game.Values.Where(x => x.GetPlayer() == player).Sum(x => x.Score)));
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
            UpdatePlot();
            Chart.Update();
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
                await _hubConnection.InvokeAsync("JoinGroup", Id);
                CreatePlot();
                ComputeTotals();
                UpdatePage();
            }
        }

        private void Send(string key, int score)
        {
            int clampedValue = Math.Clamp(score, 0, 5000);
            Game[key] = clampedValue;

            _hubConnection.InvokeAsync("UpdateValue", Id, key, clampedValue);
            ComputeTotals();
            UpdatePlot();
            Chart.Update();
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

        private void CreatePlot()
        {
            PlotConfig = new LineConfig
            {
                Options = new LineOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "Scores"
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Nearest,
                        Intersect = true
                    },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Nearest,
                        Intersect = true
                    },
                    Scales = new Scales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new CategoryAxis
                            {
                                ScaleLabel = new ScaleLabel
                                {
                                    LabelString = "Round"
                                }
                            }
                        },
                        YAxes = new List<CartesianAxis>
                        {
                            new LinearCartesianAxis
                            {
                                ScaleLabel = new ScaleLabel
                                {
                                    LabelString = "Score"
                                }
                            }
                        }
                    }
                }
            };

            foreach (string item in Game.Maps.SelectMany(x => Enumerable.Range(1, 5).Select(y => $"{x[0]}_R{y}")))
            {
                PlotConfig.Data.Labels.Add(item);
            }

            UpdatePlot();
        }

        private void UpdatePlot()
        {
            var colors = new List<Color>()
            {
                Color.FromArgb(255, 99, 132),
                Color.FromArgb(255, 159, 64),
                Color.FromArgb(255, 205, 86),
                Color.FromArgb(75, 192, 192),
                Color.FromArgb(54, 162, 235),
                Color.FromArgb(153, 102, 255),
                Color.FromArgb(201, 203, 207)
            };

            PlotConfig.Data.Datasets.Clear();

            int colorIndex = 0;
            foreach (string player in Game.Players)
            {
                int sum = 0;
                var scores = new List<int>();
                foreach (int score in Game.Values.Where(x => x.GetPlayer() == player).OrderBy(x => x.GetRound()).Select(x => x.Score).TakeWhile(x => x > 0))
                {
                    sum += score;
                    scores.Add(sum);
                }

                IDataset<int> dataset = new LineDataset<int>(scores)
                {
                    Label = player,
                    BackgroundColor = ColorUtil.FromDrawingColor(colors[colorIndex % colors.Count]),
                    BorderColor = ColorUtil.FromDrawingColor(colors[colorIndex % colors.Count]),
                    Fill = FillingMode.Disabled
                };

                PlotConfig.Data.Datasets.Add(dataset);

                colorIndex++;
            }
        }
    }
}
