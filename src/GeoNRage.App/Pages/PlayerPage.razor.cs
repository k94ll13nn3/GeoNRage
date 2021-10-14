using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Refit;

namespace GeoNRage.App.Pages;

public partial class PlayerPage
{
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
            Name = "Historique des parties"
        });
    }

    private void FilterChallengesDone(ChangeEventArgs args)
    {
        string search = args?.Value as string ?? string.Empty;
        ChallengesDone = Player.ChallengesDone.Where(c => c.MapName.Contains(search, StringComparison.OrdinalIgnoreCase));
        StateHasChanged();
    }
}
