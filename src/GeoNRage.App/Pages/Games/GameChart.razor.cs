using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;

namespace GeoNRage.App.Pages.Games;

public partial class GameChart
{
    public PlotlyChart? Chart { get; set; } = null!;

    public Config Config { get; set; } = new();

    public Layout Layout { get; set; } = new();

    public IList<ITrace> Data { get; set; } = new List<ITrace>();

    [Parameter]
    public IEnumerable<GameChallengeDto> Challenges { get; set; } = null!;

    [Parameter]
    public IEnumerable<GamePlayerDto> Players { get; set; } = null!;

    [Parameter]
    public IReadOnlyDictionary<(int challengeId, string playerId, int round), int?> Scores { get; set; } = null!;

    protected override void OnInitialized()
    {
        CreatePlot();
    }

    private void CreatePlot()
    {
        Config = new PlotlyConfig().Config;

        Layout = new PlotlyConfig().Layout;
        Layout.Height = 600;
        Layout.Width = 750;

        Data = new List<ITrace>();

        var labels = new List<object>();
        int i = 1;
        foreach (string item in Challenges.SelectMany(x => Enumerable.Range(1, 5).Select(_ => $"{x.MapName[0]}_R{i++}")).Prepend("0"))
        {
            labels.Add(item);
        }

        foreach (GamePlayerDto player in Players)
        {
            UpdatePlot(player, labels);
        }
    }

    private void UpdatePlot(GamePlayerDto player, List<object> labels)
    {
        int sum = 0;
        var scores = new List<object> { 0 };
        var values = new List<int?>();
        foreach (GameChallengeDto challenge in Challenges)
        {
            for (int i = 0; i < 5; i++)
            {
                values.Add(Scores[(challenge.Id, player.Id, i + 1)]);
            }
        }

        foreach (int? score in values.TakeWhile(x => x is not null))
        {
            sum += score ?? 0;
            scores.Add(sum);
        }

        var dataset = new Scatter
        {
            Name = player.Name,
            Mode = ModeFlag.Lines | ModeFlag.Markers,
            X = labels,
            Y = scores,
            Line = new Line { Width = 4 },
            Marker = new Marker { Size = 8 }
        };

        Data.Add(dataset);
    }
}
