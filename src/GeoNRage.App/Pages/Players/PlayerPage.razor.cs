using System.Text.RegularExpressions;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.BarLib;
using Refit;

namespace GeoNRage.App.Pages.Players;

public partial class PlayerPage
{
    private static readonly Regex NumericFilterRegex = new(@"^(?<map>.*?)\s*(?<operator>[><]=?|=){1}\s*(?<value>\d{1,5})$", RegexOptions.Compiled);

    [Parameter]
    public string Id { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    public PlayerFullDto Player { get; set; } = null!;

    public bool PlayerFound { get; set; } = true;

    public bool Loaded { get; set; }

    public PlotlyChart? Chart { get; set; } = null!;

    public Config Config { get; set; } = new();

    public Layout Layout { get; set; } = new();

    public IList<ITrace> Data { get; } = new List<ITrace>();

    public IEnumerable<PlayerChallengeDto> ChallengesDone { get; set; } = new List<PlayerChallengeDto>();

    public string Filter { get; set; } = string.Empty;

    public bool ShowWarning { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Loaded = false;
        StateHasChanged();
        ApiResponse<PlayerFullDto> response = await PlayersApi.GetFullAsync(Id);
        if (!response.IsSuccessStatusCode || response.Content is null)
        {
            PlayerFound = false;
        }
        else
        {
            Loaded = true;
            PlayerFound = true;
            Player = response.Content;
            ChallengesDone = Player.ChallengesDone;
            CreatePlot();
            StateHasChanged();
        }
    }

    internal override async void OnSettingsChanged(object? sender, EventArgs e)
    {
        await OnInitializedAsync();
    }

    private void CreatePlot()
    {
        Config = new PlotlyConfig().Config;

        Layout = new PlotlyConfig().Layout;
        Layout.Height = 250;

        Data.Clear();
        Data.Add(new Bar
        {
            X = Player.GameHistory.Where(g => g.Sum > 0).Select(g => $"G{g.GameId}" as object).ToList(),
            Y = Player.GameHistory.Where(g => g.Sum > 0).Select(g => g.Sum as object).ToList(),
            TextArray = Player.GameHistory.Where(g => g.Sum > 0).Select(g => $"{g.GameName} - {g.GameDate.ToShortDateString()} - {g.NumberOf5000} fois 5000").ToList(),
            Name = "Historique des parties",
            TextPosition = TextPositionEnum.None
        });
    }

    private void FilterChallengesDone()
    {
        ShowWarning = false;
        Match match = NumericFilterRegex.Match(Filter);
        if (match.Success && int.TryParse(match.Groups["value"].Value, out int value))
        {
            IEnumerable<PlayerChallengeDto> challenges = Player.ChallengesDone
                .Where(c => c.MapName.Contains(match.Groups["map"].Value, StringComparison.OrdinalIgnoreCase));
            challenges = match.Groups["operator"].Value switch
            {
                ">=" => challenges.Where(c => c.Sum >= value),
                "<=" => challenges.Where(c => c.Sum <= value),
                ">" => challenges.Where(c => c.Sum > value),
                "<" => challenges.Where(c => c.Sum < value),
                "=" => challenges.Where(c => c.Sum == value),
                _ => challenges,
            };

            ChallengesDone = challenges.ToList();
        }
        else
        {
            ChallengesDone = Player.ChallengesDone.Where(c => $"{c.MapName}{c.Sum}".Contains(Filter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!ChallengesDone.Any())
        {
            ChallengesDone = Player.ChallengesDone.ToList();
            ShowWarning = true;
        }

        StateHasChanged();
    }
}
