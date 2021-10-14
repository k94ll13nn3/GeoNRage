using System.Text.RegularExpressions;
using System.Timers;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Refit;

namespace GeoNRage.App.Pages;

public partial class PlayerPage
{
    private bool _disposedValue;
    private static readonly Regex NumericFilterRegex = new(@"^(?<map>.*?)\s*(?<operator>[><]?[=]?)\s*(?<value>\d{1,5})$", RegexOptions.Compiled);
    private System.Timers.Timer? _filterTimer;
    private string _filterContent = string.Empty;

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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!_disposedValue)
        {
            if (_filterTimer is not null)
            {
                _filterTimer.Elapsed -= OnUserFinish;
                _filterTimer.Dispose();
            }

            _disposedValue = true;
        }
    }

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
            _filterTimer = new(500);
            _filterTimer.Elapsed += OnUserFinish;
            _filterTimer.AutoReset = false;
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
            Name = "Historique des parties"
        });
    }

    private void OnFilterInput(ChangeEventArgs args)
    {
        if (_filterTimer is not null)
        {
            _filterTimer.Stop();
            _filterTimer.Start();
            _filterContent = args?.Value as string ?? string.Empty;
        }
    }

    private void OnUserFinish(object? source, ElapsedEventArgs e)
    {
        InvokeAsync(() => FilterChallengesDone());
    }

    private void FilterChallengesDone()
    {
        Match match = NumericFilterRegex.Match(_filterContent);
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
            ChallengesDone = Player.ChallengesDone.Where(c => c.MapName.Contains(_filterContent, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        StateHasChanged();
    }
}
