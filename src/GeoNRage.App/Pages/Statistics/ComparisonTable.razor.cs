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
        AddElement(p => p.Statistics.NumberOf5000, "Nombre de 5000", (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.NumberOf4999, "Nombre de 4999", (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.ChallengesCompleted, "Nombre de carte complétées", (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.BestGameSum, "Meilleure partie", NullableComparer);

        AddElement(p => p.Statistics.NumberOf25000, "Nombre de 25000", (i, j) => i.CompareTo(j));
        AddElement(p => p.Statistics.NumberOf0, "Nombre de 0", (i, j) => j.CompareTo(i));
        AddElement(p => p.Statistics.NumberOfTimeOut, "Nombre de time out (sans guess)", (i, j) => j.CompareTo(i));
        AddElement(p => p.Statistics.NumberOfTimeOutWithGuess, "Nombre de time out (avec guess)", (i, j) => j.CompareTo(i));
        AddElement(p => p.Statistics.Best5000Time, "5000 le plus rapide", (i, j) => NullableComparer(i, j, true), x => x.ToTimeString());
        AddElement(p => p.Statistics.Best25000Time, "25000 le plus rapide", (i, j) => NullableComparer(i, j, true), x => x.ToTimeString());

        AddElement(p => p.Statistics.TimeByRoundAverage, "Temps moyen", (i, j) => NullableComparer(i, j, true), x => x.ToTimeString());
        AddElement(p => p.Statistics.DistanceAverage, "Distance moyenne", (i, j) => NullableComparer(i, j, true), x => x.ToDistanceString());
        AddElement(p => p.Statistics.TotalTime, "Temps total", (i, j) => NullableComparer(i, j, true), x => x.ToTimeString());
        AddElement(p => p.Statistics.TotalDistance, "Distance totale", (i, j) => NullableComparer(i, j, true), x => x.ToDistanceString());

        AddElement(p => p.Statistics.MapAverage, "Moyenne par carte", NullableComparer, x => x is null ? "—" : $"{x:F1}");
        AddElement(p => p.Statistics.RoundAverage, "Moyenne par round", NullableComparer, x => x is null ? "—" : $"{x:F1}");
        AddElement(p => p.Statistics.AverageOf5000ByGame, "Moyenne de 5000 par partie", NullableComparer, x => x is null ? "—" : $"{x:F1}");
        AddElement(p => p.Statistics.GameAverage, "Moyenne par partie", NullableComparer, x => x is null ? "—" : $"{x:F1}");
    }

    private void AddElement<T>(Func<PlayerFullDto, T> selector, string title, Func<T, T, int> comparer, Func<T, string>? customFormatter = null)
    {
        Func<T, string> formatter = customFormatter ?? (t => t?.ToString() ?? "—");

        PlayersComparisons.Add(new(title, formatter(selector(Player1)), formatter(selector(Player2)), comparer(selector(Player1), selector(Player2))));
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
