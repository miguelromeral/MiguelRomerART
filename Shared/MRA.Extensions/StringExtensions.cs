using System.Text.RegularExpressions;

namespace MRA.Extensions;

public static class StringExtensions
{
    public static string GetSpotifyTrackId(this string url)
    {
        string pattern = @"\/track\/([^\/?]+)(?:\?|$)";

        Regex regex = new Regex(pattern);

        Match match = regex.Match(url ?? string.Empty);

        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}
