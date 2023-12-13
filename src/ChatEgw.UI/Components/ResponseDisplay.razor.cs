using System.Text.RegularExpressions;

namespace ChatEgw.UI.Components;

public partial class ResponseDisplay
{
    [GeneratedRegex(@"\(\s*(\d{1,3}(\s*,\s*\d{1,3})*)\s*(\)|$)", RegexOptions.Compiled | RegexOptions.Multiline)]
    protected partial Regex ReReferences();

    private string HandleReferences(Match match)
    {
        var items = match
            .Groups[1]
            .Value
            .Split(',')
            .Select(s => s.Trim())
            .Select(s => int.TryParse(s, out var i) ? i : -1)
            .Where(i => i >= 0 && i < Answers.Count)
            .Select(i =>
                Answers[i].Uri is null
                    ? "<i>" + Answers[i].ReferenceCode + "</i>"
                    : "<a href=\""
                      + Answers[i].Uri +
                      "\" target=\"_blank\" class=\"mud-link mud-primary-text mud-link-underline-hover\">" +
                      Answers[i].ReferenceCode
                      + "</a>")
            .ToArray();
        if (items.Any())
        {
            return "(" + string.Join(", ", items) + ")";
        }

        return "";
    }
}