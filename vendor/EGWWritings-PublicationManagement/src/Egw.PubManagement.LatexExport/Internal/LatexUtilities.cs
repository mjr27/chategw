using System.Text;

namespace Egw.PubManagement.LatexExport.Internal;

/// <summary>
/// Latex utilities
/// </summary>
internal static class LatexTextUtilities
{
    private static readonly Dictionary<char, string> TextQuotes = new()
    {
        ['{'] = "\\{",
        ['}'] = "\\}",
        ['\\'] = "\\textbackslash{}",
        ['#'] = "\\#",
        ['$'] = "\\$",
        ['%'] = "\\%",
        ['&'] = "\\&",
        ['^'] = "\\textasciicircum{}",
        ['\''] = "'",
        ['"'] = "\"",
        ['_'] = "\\_",
        ['~'] = "\\textasciitilde{}"
    };

    /// <summary>
    /// Escape a string for use in a latex document
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string EscapeLatex(this string s)
    {
        var sb = new StringBuilder();
        foreach (char c in s)
        {
            if (TextQuotes.TryGetValue(c, out string? replacement))
            {
                sb.Append(replacement);
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}