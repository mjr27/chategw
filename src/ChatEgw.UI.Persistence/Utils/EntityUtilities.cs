using System.Text.RegularExpressions;

namespace ChatEgw.UI.Persistence.Utils;

public partial class EntityUtilities
{
    public static string NormalizeEntityValue(string s)
    {
        return ReMultipleSpaces()
            .Replace(
                new string(s.Normalize().ToCharArray()
                    .Select(c =>
                    {
                        if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
                        {
                            return ' ';
                        }

                        return char.ToLowerInvariant(c);
                    }).ToArray()),
                " ");
    }


    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex ReMultipleSpaces();
}