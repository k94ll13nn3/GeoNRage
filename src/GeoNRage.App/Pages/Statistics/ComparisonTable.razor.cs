using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Statistics;

public partial class ComparisonTable
{
    [Parameter]
    public PlayerFullDto Player1 { get; set; } = null!;

    [Parameter]
    public PlayerFullDto Player2 { get; set; } = null!;

    public ICollection<(string statistic, string player1, string player2, int comparison)> PlayersComparisons { get; } = new List<(string statistic, string player1, string player2, int comparison)>();

    protected override void OnInitialized()
    {
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
        Func<T, string> formatter = customFormatter ?? (t => t?.ToString() ?? "—");

        Func<PlayerFullDto, T> selector = selectorExpression.Compile();

        PlayersComparisons.Add(new(
            LabelStore.Get(selectorExpression),
            formatter(selector(Player1)),
            formatter(selector(Player2)),
            comparer(selector(Player1), selector(Player2))));
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
