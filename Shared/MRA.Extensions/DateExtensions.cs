using System.Globalization;

namespace MRA.Extensions;

public static class DateExtensions
{
    public const string INPUT_DATE_FORMAT = "yyyy/MM/dd";
    public const string OUTPUT_DATE_FORMAT = "dd MMMM yyyy";

    public static string FormattedDate(this string Date)
    {
        if (string.IsNullOrEmpty(Date))
            return "";

        DateTime date = DateTime.ParseExact(Date, INPUT_DATE_FORMAT, CultureInfo.InvariantCulture);
        return FormattedDate(date);
    }

    public static string FormattedDate(this DateTime date)
    {
        var cultureInfo = CultureInfo.GetCultureInfo("es-ES");
        return date.ToString(OUTPUT_DATE_FORMAT, cultureInfo);
    }
}
