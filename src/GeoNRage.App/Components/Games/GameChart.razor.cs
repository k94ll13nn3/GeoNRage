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
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Games
{
    public partial class GameChart
    {
        private readonly List<Color> _colors = new()
        {
            Color.Aqua,
            Color.BlueViolet,
            Color.Chartreuse,
            Color.DarkGoldenrod,
            Color.DarkOrange,
            Color.HotPink,
            Color.Lime,
            Color.RoyalBlue,
            Color.Salmon,
            Color.Snow,
        };

        public LineConfig PlotConfig { get; set; } = new LineConfig
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

        public Chart Chart { get; set; } = null!;

        [Parameter]
        public GameDto Game { get; set; } = null!;

        public async Task UpdateAsync(string playerId)
        {
            UpdatePlot(Game.Players.First(x => x.Id == playerId));
            await Chart.Update();
        }

        protected override void OnInitialized()
        {
            CreatePlot();
        }

        private void CreatePlot()
        {
            foreach (string item in Game.Challenges.SelectMany(x => Enumerable.Range(1, 5).Select(y => $"{x.MapName[0]}_R{y}")).Prepend("0"))
            {
                PlotConfig.Data.Labels.Add(item);
            }

            foreach (PlayerDto player in Game.Players)
            {
                IDataset<int> dataset = new LineDataset<int>()
                {
                    Label = player.Name,
                    BackgroundColor = ColorUtil.FromDrawingColor(_colors[PlotConfig.Data.Datasets.Count % _colors.Count]),
                    BorderColor = ColorUtil.FromDrawingColor(_colors[PlotConfig.Data.Datasets.Count % _colors.Count]),
                    Fill = FillingMode.Disabled
                };

                PlotConfig.Data.Datasets.Add(dataset);
                UpdatePlot(player);
            }
        }

        private void UpdatePlot(PlayerDto player)
        {
            int sum = 0;
            var scores = new List<int> { 0 };
            var values = new List<int?>();
            foreach (ChallengeDto challenge in Game.Challenges)
            {
                for (int i = 0; i < 5; i++)
                {
                    values.Add(Game[challenge.Id, player.Id, i + 1]);
                }
            }

            foreach (int? score in values.TakeWhile(x => x is not null))
            {
                sum += score ?? 0;
                scores.Add(sum);
            }

            if (PlotConfig.Data.Datasets.FirstOrDefault(x => (x as LineDataset<int>)?.Label == player.Name) is not Dataset<int> dataset)
            {
                dataset = new LineDataset<int>()
                {
                    Label = player.Name,
                    BackgroundColor = ColorUtil.FromDrawingColor(_colors[PlotConfig.Data.Datasets.Count % _colors.Count]),
                    BorderColor = ColorUtil.FromDrawingColor(_colors[PlotConfig.Data.Datasets.Count % _colors.Count]),
                    Fill = FillingMode.Disabled
                };

                PlotConfig.Data.Datasets.Add(dataset);
            }

            dataset.Clear();
            dataset.AddRange(scores);
        }
    }
}
