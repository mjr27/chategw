using System.Text.RegularExpressions;
namespace Egw.PubManagement.Application.Extensions;

internal static class DateParser
{
    private static readonly Regex[] Regexps =
    {
        new(@"^(?<year>\d{4})"), new(@"^(?<month>\w+)\s*-\s*(\w+)\s+(?<year>\d{4})"),
        new(@"^(?<month>\w+)\s+(?<day>\d+)\s*[,-]\s*(\d+)\s*,?\s*(?<year>\d{4})"),
        new(@"^(?<month>\w+)\s+(?<day>\d+)[,]\s*(\w+)\s+(\d+)[,]\s*(?<year>\d{4})"),
        new(@"^(?<month>\w+)\s+(?<day>\d+)\s*[,]\s*(?<year>\d{4})\s+.*$"),
        new(@"^(?<month>\w+)\s+(?<year>\d{4})\s+.*$"), new(@"^(?<month>\w+)\s*-\s*(\w+)\s+(?<year>\d{4})\s+.*$"),
    };


    public static bool TryParseDate(string dateStr, out DateOnly date)
    {
        date = DateOnly.MinValue;
        if (DateOnly.TryParse(dateStr, out date))
        {
            return true;
        }


        foreach (Regex? regexp in Regexps)
        {
            if (TryExtractDate(regexp.Match(dateStr), out string? extractedDate))
            {
                if (DateOnly.TryParse(extractedDate, out date))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryExtractDate(Match match, out string formattedDate)
    {
        formattedDate = "";
        if (!match.Success)
        {
            return false;
        }

        string year = match.Groups["year"].Value;
        if (string.IsNullOrWhiteSpace("year"))
        {
            return false;
        }

        string month = match.Groups["month"].Value;
        if (string.IsNullOrWhiteSpace(month))
        {
            month = "January";
        }

        string day = match.Groups["day"].Value;
        if (string.IsNullOrWhiteSpace(day))
        {
            day = "1";
        }

        formattedDate = $"{month} {day}, {year}";
        return true;
    }
}