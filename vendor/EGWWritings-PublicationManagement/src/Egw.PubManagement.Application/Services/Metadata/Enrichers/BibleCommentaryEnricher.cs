using System.Text.RegularExpressions;

using AngleSharp.Dom;

using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Services.Metadata.Enrichers;

/// <summary>
/// Bible commentary enricher
/// </summary>
public partial class BibleCommentaryEnricher : IMetadataEnricher
{
    private readonly BibleLinkNormalizer _normalizer;
    private string _book;
    private int _chapter;
    private int[]? _verses;

    /// <summary>
    /// Default constructor
    /// </summary>
    public BibleCommentaryEnricher()
    {
        _normalizer = new BibleLinkNormalizer();
        _book = "";
        _chapter = 0;
    }

    /// <inheritdoc />
    public void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata)
    {
        switch (paragraph)
        {
            case { HeadingLevel: 2 }:
                HandleH2(paragraph, metadata);
                break;
            case { HeadingLevel: 3 }:
                HandleH3(paragraph, metadata);
                break;
            case { HeadingLevel: 0 or > 3 }:
                HandleParagraph(paragraph, metadata);
                break;
        }
    }


    private void HandleH2(Paragraph paragraph, ParagraphMetadata metadata)
    {
        string? book = ExtractBookReference(paragraph);
        if (book is not null)
        {
            _book = book;
            metadata.WithBible(_book);
        }

        _chapter = 0;
        _verses = null;
    }

    private void HandleH3(Paragraph paragraph, ParagraphMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(_book))
        {
            return;
        }

        int? chapter = ExtractChapterReference(paragraph);
        _verses = null;
        if (!chapter.HasValue)
        {
            return;
        }

        _chapter = chapter.Value;
        metadata.WithBible(_book, _chapter);
    }

    private void HandleParagraph(Paragraph paragraph, ParagraphMetadata metadata)
    {
        int[]? verses = ExtractVerses(paragraph);
        if (verses != null)
        {
            _verses = verses;
        }

        if (!string.IsNullOrWhiteSpace(_book) && _chapter > 0 && _verses is not null)
        {
            metadata.WithBible(_book, _chapter, _verses);
        }
    }

    private int[]? ExtractVerses(Paragraph paragraph)
    {
        IElement? content = paragraph.DeserializeToHtml();
        IElement? link = null;
        while (content != null)
        {
            if (content.TagName.ToUpperInvariant() == "A" &&
                content.GetAttribute("href")?.StartsWith("egw://bible") == true)
            {
                link = content;
                break;
            }

            content = content.FirstChild as IElement;
        }

        if (link?.OuterHtml == null)
        {
            return null;
        }

        int[]? verses = GetVerses(link.TextContent)
                        ?? GetVerses(link.GetAttribute("title"));
        if (verses is null)
        {
            Console.WriteLine($"Unable to get verses from {link.OuterHtml}");
        }

        return verses;
    }


    private string? ExtractBookReference(Paragraph paragraph)
    {
        IElement content = paragraph.DeserializeToHtml();
        IElement? firstLink = content.QuerySelector("a[href^=\"egw://bible\"]");
        if (firstLink is null)
        {
            return ExtractBook(content.TextContent.Trim());
        }

        return ExtractBook(firstLink.GetAttribute("title"))
               ?? ExtractBook(firstLink.TextContent.Trim());
    }


    private int? ExtractChapterReference(Paragraph paragraph)
    {
        IElement content = paragraph.DeserializeToHtml();
        IElement? firstLink = content.QuerySelector("a[href^=\"egw://bible\"]");
        if (firstLink is null)
        {
            return null;
        }

        return ExtractChapter(firstLink.TextContent.Trim())
               ?? ExtractChapter(firstLink.GetAttribute("title"))
               ?? null;
    }

    private string? ExtractBook(string? text)
    {
        if (text is null)
        {
            return null;
        }

        string link = ReStripEnd().Replace(text, "");
        string? normalized = _normalizer.NormalizeRefCode(link);
        return normalized is not null ? link : normalized;
    }

    private int? ExtractChapter(string? text)
    {
        if (text is null)
        {
            return null;
        }

        if (text.Contains(':'))
        {
            int idx = text.LastIndexOf(':');
            text = text[..idx];
        }

        string link = ReStripVerses().Replace(text, "");
        string? normalized = _normalizer.NormalizeRefCode(link);
        if (normalized is null)
        {
            return null;
        }

        Match m = ReGetVerse().Match(normalized);
        if (!m.Success)
        {
            return null;
        }

        return int.Parse(m.Groups[1].Value);
    }

    private int[]? GetVerses(string? content)
    {
        if (content is null)
        {
            return null;
        }

        content = ReFixRanges().Replace(content, match =>
        {
            int chapter1 = int.Parse(match.Groups[1].Value);
            int chapter2 = int.Parse(match.Groups[3].Value);
            int start = int.Parse(match.Groups[2].Value);
            int end = int.Parse(match.Groups[4].Value);
            return chapter1 == chapter2
                ? $"{chapter1}:{start}-{end}"
                : match.Groups[0].Value;
        });
        int separatorIndex = content.IndexOf(':');
        if (separatorIndex >= 0)
        {
            content = content[(separatorIndex + 1)..];
        }

        content = TrimStartingSpaces().Replace(content, "");
        content = content.Trim();
        return ParseVerseRange(content);
    }

    private int[]? ParseVerseRange(string content)
    {
        string parsedContent = ReSpaces().Replace(content, "")
            .TrimEnd('.')
            .Trim();

        parsedContent = ReDashes().Replace(parsedContent, match =>
        {
            int start = int.Parse(match.Groups[1].Value);
            int end = int.Parse(match.Groups[2].Value);
            return start > end ? match.Groups[0].Value : string.Join(',', Enumerable.Range(start, end - start + 1));
        });
        parsedContent = parsedContent.Replace("and", ",");
        if (!parsedContent.All(c => char.IsDigit(c) || c == ','))
        {
            return null;
        }

        var integers = parsedContent.Split(',').Select(s => int.TryParse(s, out int i) ? (int?)i : null)
            .ToList();
        int[] result = integers
            .OfType<int>()
            .OrderBy(r => r)
            .ToArray();
        return (result.Length == integers.Count && result.Length > 0) ? result : null;
    }

    [GeneratedRegex(@"(\d+)$")]
    private static partial Regex ReGetVerse();

    [GeneratedRegex(@"[-:\.\s\d]*$")]
    private static partial Regex ReStripEnd();

    [GeneratedRegex(@"[-:\.\s]*$")]
    private static partial Regex ReStripVerses();

    [GeneratedRegex(@"^[^\d]+")]
    private static partial Regex TrimStartingSpaces();

    [GeneratedRegex(@"\s+")]
    private static partial Regex ReSpaces();

    [GeneratedRegex(@"(\d+)(?:[-—‐‑‒–——−﹘﹣]|to)(\d+)")]
    private static partial Regex ReDashes();

    [GeneratedRegex(@"(\d+):(\d+)-(\d+):(\d+)")]
    private static partial Regex ReFixRanges();
}