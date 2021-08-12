using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages;

public partial class PlayersStatistics
{
    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    internal IEnumerable<PlayerStatisticDto> Players { get; set; } = Enumerable.Empty<PlayerStatisticDto>();

    protected override async Task OnInitializedAsync()
    {
        Players = await PlayersApi.GetAllStatisticsAsync();
    }

    internal override async void OnMapStatusChanged(object? sender, EventArgs e)
    {
        Players = Enumerable.Empty<PlayerStatisticDto>();
        StateHasChanged();
        Players = await PlayersApi.GetAllStatisticsAsync();
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
}
