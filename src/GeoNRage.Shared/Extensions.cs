using System.Globalization;
using System.Text;

namespace GeoNRage.Shared;

public static class Extensions
{
    public static string FormatNullWithDash<T>(this T value)
    {
        return value?.ToString() ?? "—";
    }

    public static string RemoveDiacritics(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        string normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string ToDistanceString(this double? distance)
    {
        return distance switch
        {
            < 1000 => $"{distance:F1}m",
            < 1000 * 1000 => $"{distance / 1000:F1}km",
            < 1000 * 1000 * 1000 => $"{distance / (1000 * 1000):F1}Mm",
            null => "—",
            _ => $"{distance:N1}m"
        };
    }

    public static string ToTimeString(this int? time)
    {
        if (time is null)
        {
            return "—";
        }

        var span = TimeSpan.FromSeconds(time.Value);
        return $"{(span.Duration().Days > 0 ? $"{span.Days}j " : "")}"
            + $"{(span.Duration().Hours > 0 ? $"{span.Hours}h " : "")}"
            + $"{(span.Duration().Minutes > 0 ? $"{span.Minutes}m " : "")}"
            + $"{(span.Duration().Seconds > 0 ? $"{span.Seconds}s" : "")}";
    }

    public static string ToTimeString(this int time)
    {
        return ((int?)time).ToTimeString();
    }

    public static string ToTimeString(this double? time)
    {
        return ((int?)time).ToTimeString();
    }
}
