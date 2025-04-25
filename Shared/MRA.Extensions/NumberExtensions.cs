namespace MRA.Extensions;

public static class NumberExtensions
{
    public const string SUFFIX_HUMAN_THOUSANDS = "k";
    public const string SUFFIX_HUMAN_MILLIONS = "M";

    public const string TIME_SUFFIX_HOUR = "h";
    public const string TIME_SUFFIX_MIN = "min";
    public const string TIME_SUFFIX_NOT_AVAILABLE = "N/A";

    public static string HumanFormat(this long number)
    {
        const long ONE_MILLION = 1000000;
        const long ONE_THOUSAND = 1000;

        if (number < ONE_THOUSAND)
        {
            return number.ToString();
        }
        else if (number < ONE_MILLION)
        {
            double valorFormateado = Math.Round((double)number / ONE_THOUSAND, 1);
            return $"{valorFormateado} {SUFFIX_HUMAN_THOUSANDS}";
        }
        else
        {
            double valorFormateado = Math.Round((double)number / ONE_MILLION, 1);
            return $"{valorFormateado} {SUFFIX_HUMAN_MILLIONS}";
        }
    }

    public static string GetHumanTime(this int Time)
    {
        if (Time > 0)
        {
            int hours = Time / 60;
            int remainingMinutes = Time % 60;
            var tokens = new List<string>();

            if (hours > 0)
            {
                tokens.Add($"{hours}{TIME_SUFFIX_HOUR}");
            }
            if (remainingMinutes > 0)
            {
                tokens.Add($"{remainingMinutes}{TIME_SUFFIX_MIN}");
            }

            return string.Join(" ", tokens);
        }
        else
        {
            return TIME_SUFFIX_NOT_AVAILABLE;
        }
    }
}
