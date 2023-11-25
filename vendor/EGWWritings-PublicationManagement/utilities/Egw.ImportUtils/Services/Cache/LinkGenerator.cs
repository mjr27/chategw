using System.Globalization;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using Egw.PubManagement.LegacyImport.BibleCodes;

namespace Egw.ImportUtils.Services.Cache;

public class LinkGenerator
{
    private readonly BibleLinkNormalizer _normalizer;
    private readonly HtmlParser _parser;

    public LinkGenerator(BibleLinkNormalizer normalizer)
    {
        _normalizer = normalizer;
        _parser = new HtmlParser(new HtmlParserOptions
        {
            IsScripting = false,
            IsStrictMode = false,
            IsKeepingSourceReferences = false,
            IsAcceptingCustomElementsEverywhere = true,
            IsPreservingAttributeNames = false
        });
    }

    public IEnumerable<LinkGeneratorParagraphLinks> GenerateLinksFor(
        LinkGeneratorBookDto book,
        IEnumerable<LinkGeneratorParagraph> paragraphs)
    {
        string pubTypeLower = book.PubType.ToLower();
        int idElement = 0;
        var nextRefCodes = new List<string>();
        foreach (LinkGeneratorParagraph paragraph in paragraphs)
        {
            using IHtmlDocument doc = _parser.ParseDocument(paragraph.Content);
            if (paragraph.Property1 == "page")
            {
                var pageLinks = doc.Descendants<IElement>()
                    .Where(r => r.NodeName == "A" && r.HasAttribute("name"))
                    .Select(link => book.PubCode + "#" + link.GetAttribute("name")!.TrimStart('#'))
                    .ToList();
                nextRefCodes.AddRange(pageLinks);
                continue;
            }

            idElement = paragraph.IsContinuation == "false"
                ? paragraph.IdElement
                : idElement;

            IEnumerable<string> refCodes = pubTypeLower switch
            {
                "book"
                    or "letter"
                    or "study guide"
                    or "sermon" => BuildBookRefCodes(book.PubCode, paragraph.Ref2, paragraph.Ref3),
                "bible" => BuildBibleRefCodes(book.PubCode, paragraph.Element, paragraph.Ref2, paragraph.Ref3, paragraph.Ref4),
                "periodical" when book.SubType == "no pagebreaks" => BuildPeriodicalNoPageBreaksRefCodes(
                    book.PubCode,
                    paragraph.Ref2,
                    paragraph.Ref4,
                    paragraph.Ref5,
                    paragraph.Ref6,
                    paragraph.Ref7),
                "periodical" when string.IsNullOrWhiteSpace(book.SubType) => BuildPeriodicalPageBreaksRefCodes(
                    book.PubCode,
                    paragraph.Ref2,
                    paragraph.Ref3,
                    paragraph.Ref4,
                    paragraph.Ref5,
                    paragraph.Ref6),
                "ltms" => BuildLtMsRefCodes(paragraph.Ref4, paragraph.Ref7),
                "scriptindex" or "topicalindex" or "dictionary" => ArraySegment<string>.Empty,
                _ => throw new ArgumentException($"Bad publication type {book.PubType}.{book.SubType}")
            };
            var refCodeSet = refCodes.ToList();
            if (nextRefCodes.Count != 0)
            {
                refCodeSet.AddRange(nextRefCodes);
                nextRefCodes.Clear();
            }

            IEnumerable<IElement> links = doc.Descendants<IElement>().Where(r => r.NodeName == "A" && r.HasAttribute("name"));
            refCodeSet.AddRange(links.Select(link => book.PubCode + "#" + link.GetAttribute("name")!.TrimStart('#')));
            if (refCodeSet.Any())
            {
                yield return new LinkGeneratorParagraphLinks(book.Language, book.BookId, idElement, refCodeSet);
            }
        }
    }

    private static IEnumerable<string> BuildLtMsRefCodes(string refCode4, string refCode7)
    {
        if (string.IsNullOrWhiteSpace(refCode4))
        {
            yield break;
        }

        int year = int.Parse(refCode4[..4]);
        bool isManuscript = refCode4[4] == '1';
        int letterNumber = int.Parse(refCode4[8..]);
        int number = int.Parse(refCode4.Substring(5, 3));
        string ltPart = (isManuscript ? "Ms" : "Lt")
                        + " "
                        + number
                        + (letterNumber == 0 ? "" : new string((char)('a' + letterNumber - 1), 1))
                        + " "
                        + year;
        yield return ltPart;
        if (!string.IsNullOrWhiteSpace(refCode7) && refCode7 != "0")
        {
            yield return $"{ltPart} {refCode7}";
        }
    }


    private static IEnumerable<string> BuildPeriodicalPageBreaksRefCodes(string code,
        string refCode2,
        string refCode3,
        string refCode4,
        string refCode5,
        string refCode6)
    {
        yield return code;
        if (string.IsNullOrWhiteSpace(refCode4) || string.IsNullOrWhiteSpace(refCode5) || string.IsNullOrWhiteSpace(refCode6))
        {
            yield break;
        }

        if (refCode5.Contains('-'))
        {
            refCode5 = refCode5.Split('-')[0];
        }

        foreach (string dateString in GetPeriodicalDates(refCode4, refCode5, refCode6))
        {
            yield return $"{code} {dateString}";
            if (string.IsNullOrWhiteSpace(refCode2))
            {
                continue;
            }

            yield return $"{code} {dateString} {refCode2}";


            if (string.IsNullOrWhiteSpace(refCode3))
            {
                continue;
            }

            yield return $"{code} {dateString} {refCode2} {refCode3}";
        }
    }

    private static IEnumerable<string> BuildPeriodicalNoPageBreaksRefCodes(string code,
        string refCode2,
        string refCode4,
        string refCode5,
        string refCode6,
        string refCode7)
    {
        yield return code;
        if (string.IsNullOrWhiteSpace(refCode4) || string.IsNullOrWhiteSpace(refCode5) || string.IsNullOrWhiteSpace(refCode6))
        {
            yield break;
        }

        if (refCode5.Contains('-'))
        {
            refCode5 = refCode5.Split('-')[0];
        }

        foreach (string dateString in GetPeriodicalDates(refCode4, refCode5, refCode6))
        {
            yield return $"{code} {dateString}";

            if (!string.IsNullOrWhiteSpace(refCode2))
            {
                yield return $"{code} {dateString} {refCode2}";
            }

            if (string.IsNullOrEmpty(refCode7))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(refCode2))
            {
                yield return $"{code} {dateString} {refCode2}";
                yield return $"{code} {dateString} {refCode2} {refCode7}";
                continue;
            }

            yield return $"{code} {dateString} {refCode7}";
            yield return $"{code} {dateString} A";
            yield return $"{code} {dateString} A {refCode7}";
        }
    }

    private static IEnumerable<string> GetPeriodicalDates(string refCode4, string refCode5, string refCode6)
    {
        int year = int.Parse(refCode4);
        int month = DateTime.Parse($"{refCode5} 1, 1900", CultureInfo.InvariantCulture).Month;
        int day = int.Parse(refCode6);

        string dt1 = new DateOnly(year, month, day).ToString("MM dd yyyy");
        string dt2 = new DateOnly(year, month, day).ToString("M d yyyy");

        return dt1 == dt2
            ? new[] { dt1 }
            : new[] { dt1, dt2 };
    }

    private IEnumerable<string> BuildBibleRefCodes(string code, string element, string refCode2, string refCode3,
        string refCode4)
    {
        if (element.ToLowerInvariant() == "h4")
        {
            yield break;
        }

        yield return code;
        if (string.IsNullOrWhiteSpace(refCode2))
        {
            yield break;
        }

        string bibleCode = _normalizer.NormalizeRefCode(refCode2);

        yield return bibleCode;
        yield return $"{code} {bibleCode}";
        if (string.IsNullOrWhiteSpace(refCode3) || refCode3 == "0")
        {
            yield break;
        }

        yield return $"{bibleCode} {refCode3}";
        yield return $"{code} {bibleCode} {refCode3}";

        if (string.IsNullOrWhiteSpace(refCode4) || refCode4 == "0")
        {
            yield break;
        }

        yield return $"{bibleCode} {refCode3} {refCode4}";
        yield return $"{code} {bibleCode} {refCode3} {refCode4}";
    }

    private static IEnumerable<string> BuildBookRefCodes(string publicationCode, string refCode2, string refCode3)
    {
        yield return publicationCode;
        if (string.IsNullOrWhiteSpace(refCode2))
        {
            yield break;
        }

        yield return $"{publicationCode} {refCode2}";
        if (!string.IsNullOrWhiteSpace(refCode3) && refCode3 != "0")
        {
            yield return $"{publicationCode} {refCode2} {refCode3}";
        }
    }
}