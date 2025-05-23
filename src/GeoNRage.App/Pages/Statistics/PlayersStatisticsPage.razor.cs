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
    public ModalService ModalService { get; set; } = null!;

    public IList<string> SelectedPlayerIds { get; } = [];

    internal IEnumerable<PlayerStatisticDto> Players { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Players = [.. (await PlayersApi.GetAllStatisticsAsync()).Where(p => p.BestGameSum is not null)];
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        if (e.ChangedKey != nameof(UserSettings.AllMaps))
        {
            return;
        }

        Players = [];
        StateHasChanged();
        Players = [.. (await PlayersApi.GetAllStatisticsAsync()).Where(p => p.BestGameSum is not null)];
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
        await ModalService.DisplayModalAsync<ComparisonTable>(new()
        {
            [nameof(ComparisonTable.Player1Id)] = SelectedPlayerIds[0],
            [nameof(ComparisonTable.Player2Id)] = SelectedPlayerIds[1],
        },
        ModalOptions.Default with { ShowCloseButton = true });
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
        else
        {
            SelectedPlayerIds.Remove(selectedId);
        }

        StateHasChanged();
    }
}
