using System.Linq.Expressions;
using GeoNRage.Shared.Dtos.Players;

namespace GeoNRage.Shared;

public static class LabelStore
{
    private static readonly Dictionary<string, Dictionary<string, string>> Labels = new()
    {
        [typeof(PlayerFullStatisticDto).ToString()] = new()
        {
            [nameof(PlayerFullStatisticDto.NumberOf5000)] = "Nombre de 5000",
            [nameof(PlayerFullStatisticDto.NumberOf4999)] = "Nombre de 4999",
            [nameof(PlayerFullStatisticDto.ChallengesCompleted)] = "Nombre de cartes complétées",
            [nameof(PlayerFullStatisticDto.BestGameSum)] = "Meilleure partie",
            [nameof(PlayerFullStatisticDto.NumberOf25000)] = "Nombre de 25000",
            [nameof(PlayerFullStatisticDto.NumberOf0)] = "Nombre de 0",
            [nameof(PlayerFullStatisticDto.NumberOfTimeOut)] = "Nombre de time out (sans guess)",
            [nameof(PlayerFullStatisticDto.NumberOfTimeOutWithGuess)] = "Nombre de time out (avec guess)",
            [nameof(PlayerFullStatisticDto.Best5000Time)] = "5000 le plus rapide",
            [nameof(PlayerFullStatisticDto.Best25000Time)] = "25000 le plus rapide",
            [nameof(PlayerFullStatisticDto.TimeByRoundAverage)] = "Temps moyen",
            [nameof(PlayerFullStatisticDto.DistanceAverage)] = "Distance moyenne",
            [nameof(PlayerFullStatisticDto.TotalTime)] = "Temps total",
            [nameof(PlayerFullStatisticDto.TotalDistance)] = "Distance totale",
            [nameof(PlayerFullStatisticDto.MapAverage)] = "Moyenne par carte",
            [nameof(PlayerFullStatisticDto.RoundAverage)] = "Moyenne par round",
            [nameof(PlayerFullStatisticDto.AverageOf5000ByGame)] = "Moyenne de 5000 par partie",
            [nameof(PlayerFullStatisticDto.GameAverage)] = "Moyenne par partie",
            [nameof(PlayerFullStatisticDto.NumberOfGamesPlayed)] = "Nombre de parties jouées",
            [nameof(PlayerFullStatisticDto.NumberOfFirstPlaceInGame)] = "🥇 Nombre parties gagnées",
            [nameof(PlayerFullStatisticDto.NumberOfSecondPlaceInGame)] = "🥈 Nombre de deuxième places",
            [nameof(PlayerFullStatisticDto.NumberOfThirdPlaceInGame)] = "🥉 Nombre de troisième places",
        },
        [typeof(PlayerStatisticDto).ToString()] = new()
        {
            [nameof(PlayerStatisticDto.NumberOf5000)] = "Nombre de 5000",
            [nameof(PlayerStatisticDto.NumberOf4999)] = "Nombre de 4999",
            [nameof(PlayerStatisticDto.ChallengesCompleted)] = "Nombre de cartes complétées",
            [nameof(PlayerStatisticDto.BestGameSum)] = "Meilleure partie",
            [nameof(PlayerStatisticDto.RoundAverage)] = "Moyenne par round",
        },
    };

    public static string Get<TType, TProperty>(Expression<Func<TType, TProperty>> expression)
    {
        return GetInternal(expression);
    }

    public static string Get<TProperty>(Expression<Func<TProperty>> expression)
    {
        return GetInternal(expression);
    }

    private static string GetInternal(LambdaExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        MemberExpression? memberExpression = expression.Body as MemberExpression;

        string? propertyName = memberExpression?.Member.Name;
        string? propertyType = (((memberExpression?.Expression as MemberExpression)?.Type)
            ?? ((memberExpression?.Expression as ParameterExpression)?.Type))?.ToString();

        if (propertyType is not null
            && propertyName is not null
            && Labels.TryGetValue(propertyType, out Dictionary<string, string>? typeLabels) && typeLabels.TryGetValue(propertyName, out string? label))
        {
            return label;
        }

        throw new InvalidOperationException($"No label set for property {propertyName} on type {propertyType}");
    }
}
