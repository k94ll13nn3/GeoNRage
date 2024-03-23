using System.Linq.Expressions;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Statistics;

public partial class ComparisonTable : IModal
{
    private PlayerFullDto? _player1;
    private PlayerFullDto? _player2;

    public string Id => nameof(ComparisonTable);

    [Parameter]
    public string Player1Id { get; set; } = null!;

    [Parameter]
    public string Player2Id { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    public ICollection<(string statistic, string player1, string player2, int comparison)> PlayersComparisons { get; } = [];

    protected override async Task OnInitializedAsync()
    {
        _player1 = (await PlayersApi.GetFullAsync(Player1Id)).Content;
        _player2 = (await PlayersApi.GetFullAsync(Player2Id)).Content;

        if (_player1 is null || _player2 is null)
        {
            throw new InvalidOperationException("Cannot load players for comparison.");
        }

        AddElement(p => p.Statistics.NumberOf5000, (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.NumberOf4999, (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.ChallengesCompleted, (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.BestGameSum, NullableComparer);

        AddElement(p => p.Statistics.NumberOf25000, (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.NumberOf0, (i, j) => j.CompareTo(i));
        AddElement(p => p.Statistics.NumberOfTimeOut, (i, j) => j.CompareTo(i));
        AddElement(p => p.Statistics.NumberOfTimeOutWithGuess, (i, j) => j.CompareTo(i));
        AddElement(p => p.Statistics.Best5000Time, (i, j) => NullableComparer(i, j, true), x => x.ToTimeString());
        AddElement(p => p.Statistics.Best25000Time, (i, j) => NullableComparer(i, j, true), x => x.ToTimeString());

        AddElement(p => p.Statistics.TimeByRoundAverage, (i, j) => NullableComparer(i, j, true), x => x.ToTimeString());
        AddElement(p => p.Statistics.DistanceAverage, (i, j) => NullableComparer(i, j, true), x => x.ToDistanceString());
        AddElement(p => p.Statistics.TotalTime, (i, j) => NullableComparer(i, j, true), x => x.ToTimeString());
        AddElement(p => p.Statistics.TotalDistance, (i, j) => NullableComparer(i, j, true), x => x.ToDistanceString());

        AddElement(p => p.Statistics.MapAverage, NullableComparer, x => x is null ? "—" : $"{x:F1}");
        AddElement(p => p.Statistics.RoundAverage, NullableComparer, x => x is null ? "—" : $"{x:F1}");
        AddElement(p => p.Statistics.AverageOf5000ByGame, NullableComparer, x => x is null ? "—" : $"{x:F1}");
        AddElement(p => p.Statistics.GameAverage, NullableComparer, x => x is null ? "—" : $"{x:F1}");

        AddElement(p => p.Statistics.NumberOfGamesPlayed, (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.NumberOfFirstPlaceInGame, (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.NumberOfSecondPlaceInGame, (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.NumberOfThirdPlaceInGame, (i, j) => i.CompareTo(j));
    }

    private void AddElement<T>(Expression<Func<PlayerFullDto, T>> selectorExpression, Func<T, T, int> comparer, Func<T, string>? customFormatter = null)
    {
        if (_player1 is null || _player2 is null)
        {
            throw new InvalidOperationException("Cannot load players for comparison.");
        }

        Func<T, string> formatter = customFormatter ?? (t => t?.ToString() ?? "—");

        Func<PlayerFullDto, T> selector = selectorExpression.Compile();

        PlayersComparisons.Add(new(
            LabelStore.Get(selectorExpression),
            formatter(selector(_player1)),
            formatter(selector(_player2)),
            comparer(selector(_player1), selector(_player2))));
    }

    private static int NullableComparer<T>(T? firstValue, T? secondValue) where T : struct, IComparable
    {
        return NullableComparer(firstValue, secondValue, false);
    }

    private static int NullableComparer<T>(T? firstValue, T? secondValue, bool invert) where T : struct, IComparable
    {
        return (firstValue.HasValue, secondValue.HasValue) switch
        {
            (true, true) => !invert ? firstValue!.Value.CompareTo(secondValue!.Value) : secondValue!.Value.CompareTo(firstValue!.Value),
            (true, false) => 1,
            (false, true) => -1,
            _ => 0,
        };
    }
}
