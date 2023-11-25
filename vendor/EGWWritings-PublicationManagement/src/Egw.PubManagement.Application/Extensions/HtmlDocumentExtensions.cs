using System.Text.RegularExpressions;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace Egw.PubManagement.Application.Extensions;

/// <summary>
/// HTML Document Extensions
/// </summary>
public static partial class HtmlDocumentExtensions
{
    /// <summary>
    /// Fixes the imported html document
    /// </summary>
    /// <param name="document">HTML Document</param>
    public static void FixImportedHtml(this IHtmlDocument document)
    {
        var textNodes = document.Descendents().Where(x => x.NodeType == NodeType.Text)
            .ToList();
        foreach (INode node in textNodes)
        {
            bool convertNeeded = node.TextContent.Contains('\n');
            if (convertNeeded){
                Console.WriteLine($"Before : {node.TextContent}");
            }
            // remove multiple whitespace characters in a row and replace with a single space
            node.TextContent = ReMultipleSpaces().Replace(node.TextContent, " ");
            if (convertNeeded){
                Console.WriteLine($"After : {node.TextContent}");
            }
        
        }
    }

    [GeneratedRegex("\\s+", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex ReMultipleSpaces();
}