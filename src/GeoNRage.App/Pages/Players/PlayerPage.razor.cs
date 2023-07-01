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
    private IEnumerable<string> _tags = new List<string>();

    [Parameter]
    public string Id { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    public PlayerFullDto Player { get; set; } = null!;

    public bool PlayerFound { get; set; } = true;

    public bool Loaded { get; set; }

    public PlotlyChart? Chart { get; set; }

    public Config Config { get; set; } = new();

    public Layout Layout { get; set; } = new();

    public IList<ITrace> Data { get; } = new List<ITrace>();

    public IEnumerable<PlayerChallengeDto> ChallengesDone { get; set; } = new List<PlayerChallengeDto>();

    public int SortDirection { get; set; }

    public IEnumerable<PlayerGameDto> GameHistory { get; set; } = new List<PlayerGameDto>();

    public bool IsFiltered { get; set; }

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
            GameHistory = Player.GameHistory;
            CreatePlot();
            StateHasChanged();
        }
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        if (e.ChangedKey != nameof(UserSettings.AllMaps))
        {
            return;
        }

        await OnInitializedAsync();
    }

    private void CreatePlot()
    {
        Config = new PlotlyConfig().Config;

        Layout = new PlotlyConfig().Layout;
        Layout.Height = 250;

        UpdateChartData();
    }

    private void UpdateChartData()
    {
        Data.Clear();
        Data.Add(new Bar
        {
            X = GameHistory.Where(g => g.Sum > 0).Select(g => $"G{g.GameId}" as object).ToList(),
            Y = GameHistory.Where(g => g.Sum > 0).Select(g => g.Sum as object).ToList(),
            TextArray = GameHistory.Where(g => g.Sum > 0).Select(g => $"{g.GameName} - {g.GameDate.ToShortDateString()} - {g.NumberOf5000} fois 5000").ToList(),
            Name = "Historique des parties",
            TextPosition = TextPositionEnum.None
        });
    }

    private async Task ShowFilterPanelAsync()
    {
        _tags = await ModalService.DisplayModalAsync<FilterPanel, IEnumerable<string>>(new() { [nameof(FilterPanel.Tags)] = _tags }, ModalOptions.Default);
        OnFilter();
    }

    private void OnFilter()
    {
        IsFiltered = true;
        if (_tags.Any())
        {
            var challengesDone = new List<PlayerChallengeDto>();
            foreach (string tag in _tags)
            {
                challengesDone = challengesDone.Union(FilterChallengesDone(tag)).ToList();
            }

            ChallengesDone = challengesDone;
            if (!ChallengesDone.Any())
            {
                IsFiltered = false;
                ChallengesDone = Player.ChallengesDone.ToList();
                ToastService.DisplayToast("La recherche n'a pas retournée de résultats", TimeSpan.FromSeconds(3), ToastType.Warning, "challenges-filter-no-results", true);
            }
        }
        else
        {
            IsFiltered = false;
            ChallengesDone = Player.ChallengesDone.ToList();
        }

        StateHasChanged();
    }

    private IEnumerable<PlayerChallengeDto> FilterChallengesDone(string filter)
    {
        Match match = NumericFilterRegex().Match(filter);
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

            return challenges.ToList();
        }

        return Player.ChallengesDone.Where(c => $"{c.MapName}{c.Sum}".Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private async Task SortGameHistoryAsync()
    {
        SortDirection = (SortDirection + 1) % 3;
        GameHistory = SortDirection switch
        {
            1 => Player.GameHistory.OrderBy(g => g.Sum),
            2 => Player.GameHistory.OrderByDescending(g => g.Sum),
            _ => Player.GameHistory,
        };

        UpdateChartData();
        if (Chart is not null)
        {
            await Chart.React();
        }

        StateHasChanged();
    }

    private void NavigateToGame(object x, object _)
    {
        NavigationManager.NavigateTo($"/games/{x.ToString()![1..]}");
    }

    [GeneratedRegex("^(?<map>.*?)\\s*(?<operator>[><]=?|=){1}\\s*(?<value>\\d{1,5})$", RegexOptions.Compiled)]
    private static partial Regex NumericFilterRegex();
}
