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
}
