using System.Text.RegularExpressions;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;

namespace ChatEgw.UI.Indexer.LinkExporter;

public record OutputRow(string Content, List<Range> Links);

public partial class LinkExtractor
{
    private record ProcessedContent(string Content, Dictionary<string, string> Links);

    private readonly IElement _root;

    public LinkExtractor()
    {
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = context.OpenNewAsync().Result;
        _root = document.CreateElement("div");
    }

    private const string PunctuationWhitelist = ".,;:!?'-()\"";

    private static readonly Dictionary<char, char> CharacterReplacement = new()
    {
        ['«'] = '"',
        ['»'] = '"',
        ['“'] = '"',
        ['”'] = '"',
        ['„'] = '"',
        ['‟'] = '"',
        ['‘'] = '\'',
        ['’'] = '\'',
        ['‚'] = '\'',
        ['‛'] = '\''
    };

    private static string CleanPunctuation(string text)
    {
        return new string(text
            .Select(c =>
            {
                if (CharacterReplacement.TryGetValue(c, out char replacement))
                {
                    return replacement;
                }

                return char.IsPunctuation(c)
                    ? PunctuationWhitelist.Contains(c)
                        ? c
                        : ' '
                    : c;
            })
            .ToArray());
    }

    // ReSharper disable once CognitiveComplexity
    public IEnumerable<OutputRow> ParseContent(string content)
    {
        var links = new List<Range>();
        foreach (string textBlock in ExtractFootnotes(content))
        {
            links.Clear();
            ProcessedContent processedItem = ExtractLinks(textBlock);
            string text = processedItem.Content;
            var data = processedItem.Links
                .Select(r => new
                {
                    Id = r.Key,
                    Text = r.Value,
                    Order = processedItem.Content.IndexOf(r.Key, StringComparison.Ordinal)
                })
                .OrderBy(r => r.Order)
                .ToArray();
            foreach (var row in data)
            {
                int order = text.IndexOf(row.Id, StringComparison.Ordinal);
                text = text[..order]
                       + row.Text
                       + text[(order + row.Id.Length)..];
                if (!string.IsNullOrWhiteSpace(row.Text))
                {
                    links.Add(new Range(order, order + row.Text.Length));
                }
            }

            text = ReTrimSpaces().Replace(text, MatchEvaluator);

            if (!string.IsNullOrWhiteSpace(text))
            {
                yield return new OutputRow(text, links);
            }
        }

        yield break;

        string MatchEvaluator(Match match)
        {
            int start = match.Index;

            bool isMiddle = match.Groups[3].Success;
            int decrease = match.Length - (isMiddle ? 1 : 0);

            for (var i = 0; i < links.Count; i++)
            {
                Range link = links[i];

                if (link.Start.Value > start)
                {
                    links[i] = new Range(link.Start.Value - decrease, link.End.Value - decrease);
                }
            }

            return isMiddle ? " " : "";
        }
    }

    private static ProcessedContent ExtractLinks(string content)
    {
        var links = new List<Range>();
        MatchCollection matches = ReLink().Matches(content);

        for (var i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            links.Add(new Range(match.Index, match.Index + match.Length));
        }

        content = ReTags().Replace(content, match => new string(' ', match.Length));
        content = ReHtmlEntity().Replace(content, match =>
        {
            string raw = match.Groups[0].Value;
            string encoded = HttpUtility.HtmlDecode(raw);
            return encoded + new string(' ', raw.Length - encoded.Length);
        });
        content = CleanPunctuation(content);
        if (links.Count == 0)
        {
            return new ProcessedContent(CleanText(content), new Dictionary<string, string>());
        }

        var result = new Dictionary<string, string>();
        foreach (Range link in links.AsEnumerable().Reverse())
        {
            var id = Guid.NewGuid().ToString("N");
            result[id] = CleanText(content[link]);
            content = content[..link.Start.Value]
                      + ' '
                      + id
                      + ' '
                      + content[link.End.Value..];
        }


        return new ProcessedContent(content, result);
    }

    private static string CleanText(string text)
    {
        return ReMultipleSpaces().Replace(text, " ").Trim();
    }


    private IEnumerable<string> ExtractFootnotes(string content)
    {
        content = new string(content.Select(c => char.IsWhiteSpace(c) ? ' ' : c).ToArray());
        _root.InnerHtml = content;
        IElement[] footnotes = _root.QuerySelectorAll("w-note").ToArray();
        foreach (IElement footnote in footnotes)
        {
            yield return footnote.QuerySelector("w-note-body")!.InnerHtml;
            footnote.Remove();
        }

        yield return _root.InnerHtml;
    }

    [GeneratedRegex("<a href[^>]+>(.*?)</a>", RegexOptions.Compiled)]
    private static partial Regex ReLink();

    [GeneratedRegex("<[^>]+>", RegexOptions.Compiled)]
    private static partial Regex ReTags();

    [GeneratedRegex("&.*?;", RegexOptions.Compiled)]
    private static partial Regex ReHtmlEntity();

    [GeneratedRegex(@"\s{2,}", RegexOptions.Compiled)]
    private static partial Regex ReMultipleSpaces();

    [GeneratedRegex(@"(?:(\s+$)|(^\s+)|(\s{2,}))",
        RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.Singleline)]
    private static partial Regex ReTrimSpaces();
}