using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Statistics;

public partial class PlayersStatisticsPage
{
    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public PopupService PopupService { get; set; } = null!;

    public IList<string> SelectedPlayerIds { get; } = new List<string>();

    public bool ShowComparison { get; set; }

    public PlayerFullDto Player1 { get; set; } = null!;

    public PlayerFullDto Player2 { get; set; } = null!;

    internal IEnumerable<PlayerStatisticDto> Players { get; set; } = Enumerable.Empty<PlayerStatisticDto>();

    protected override async Task OnInitializedAsync()
    {
        Players = (await PlayersApi.GetAllStatisticsAsync()).Where(p => p.BestGameSum is not null).ToList();
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        if (e.ChangedKey != nameof(UserSettings.AllMaps))
        {
            return;
        }

        Players = Enumerable.Empty<PlayerStatisticDto>();
        StateHasChanged();
        Players = (await PlayersApi.GetAllStatisticsAsync()).Where(p => p.BestGameSum is not null).ToList();
        StateHasChanged();
    }

    private static IEnumerable<PlayerStatisticDto> Sort(IEnumerable<PlayerStatisticDto> players, string column, bool ascending)
    {
        return column switch
        {
            nameof(PlayerStatisticDto.Name) => ascending ? players.OrderBy(p => p.Name) : players.OrderByDescending(p => p.Name),
            nameof(PlayerStatisticDto.NumberOf5000) => ascending ? players.OrderBy(p => p.NumberOf5000) : players.OrderByDescending(p => p.NumberOf5000),
            nameof(PlayerStatisticDto.NumberOf4999) => ascending ? players.OrderBy(p => p.NumberOf4999) : players.OrderByDescending(p => p.NumberOf4999),
            nameof(PlayerStatisticDto.ChallengesCompleted) => ascending ? players.OrderBy(p => p.ChallengesCompleted) : players.OrderByDescending(p => p.ChallengesCompleted),
            nameof(PlayerStatisticDto.BestGameSum) => ascending ? players.OrderBy(p => p.BestGameSum) : players.OrderByDescending(p => p.BestGameSum),
            nameof(PlayerStatisticDto.RoundAverage) => ascending ? players.OrderBy(p => p.RoundAverage) : players.OrderByDescending(p => p.RoundAverage),
            _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
        };
    }

    private async Task CompareAsync()
    {
        PopupService.DisplayLoader("Chargement...");
        Player1 = (await PlayersApi.GetFullAsync(SelectedPlayerIds[0])).Content!;
        Player2 = (await PlayersApi.GetFullAsync(SelectedPlayerIds[1])).Content!;
        PopupService.HidePopup();
        ShowComparison = true;
    }

    private bool CanCompare()
    {
        return SelectedPlayerIds.Count == 2;
    }

    private void PlayerChecked(string selectedId, object? value)
    {
        if ((bool?)value == true)
        {
            if (!SelectedPlayerIds.Contains(selectedId))
            {
                SelectedPlayerIds.Add(selectedId);
            }
        }
        else if (SelectedPlayerIds.Contains(selectedId))
        {
            SelectedPlayerIds.Remove(selectedId);
        }

        StateHasChanged();
    }
}
