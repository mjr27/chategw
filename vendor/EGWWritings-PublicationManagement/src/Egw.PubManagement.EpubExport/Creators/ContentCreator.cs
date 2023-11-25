using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.EpubExport.Types;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Fizzler.Systems.HtmlAgilityPack;

using HtmlAgilityPack;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Serialization;

namespace Egw.PubManagement.EpubExport.Creators;

/// <summary>
/// Content creator
/// </summary>
internal class ContentCreator
{
    private readonly PublicationDbContext _db;
    private readonly TemplateService _template;
    private readonly ContentDbWemlSerializer _wemlToHtml;
    private int _chapterFootnoteIdx = 1;
    private string _bibleBook = "";
    private readonly HtmlParser _htmlParser;
    private readonly WemlDeserializer _wemlDeserializer;
    private readonly IHtmlDocument _document;

    /// <summary> Default constructor </summary>
    public ContentCreator(PublicationDbContext db, TemplateService template)
    {
        _db = db;
        _template = template;
        _wemlToHtml = new ContentDbWemlSerializer();
        _htmlParser = new HtmlParser();
        _wemlDeserializer = new WemlDeserializer();
        _document = (IHtmlDocument)BrowsingContext.New().OpenNewAsync().Result;
    }

    /// <summary>
    /// Creates the content files
    /// </summary>
    public async Task<bool> Create(Publication publication, List<EpubChapter> chapters,
        CancellationToken ct)
    {
        foreach (EpubChapter chapter in chapters)
        {
            if (chapter is { Order: 0, EndOrder: 0 })
            {
                continue;
            }

            List<Paragraph> paragraphs = await GetChapterParagraphs(chapter, ct);
            foreach (Paragraph paragraph in paragraphs)
            {
                IElement htmlNode = _htmlParser.ParseFragment(paragraph.Content, _document.Body!).OfType<IElement>()
                    .Single();
                IWemlNode wemlNode = _wemlDeserializer.Deserialize(htmlNode);
                paragraph.Content = _wemlToHtml.Serialize(publication.Type, wemlNode).ToHtml();
                paragraph.Content = ParseParagraphContent(publication, paragraph, chapter);
            }

            await CreateFile(publication, paragraphs, chapter);
            _chapterFootnoteIdx++;
        }

        return true;
    }

    private async Task CreateFile(Publication publication, List<Paragraph> paragraphs,
        EpubChapter chapter)
    {
        var model = new { chapter, publicationTitle = publication.Title, paragraphs };
        await _template.RenderToFile(
            $"EPUB/xhtml/{chapter.File}",
            "content",
            model
        );
    }

    private async Task<List<Paragraph>> GetChapterParagraphs(EpubChapter chapter, CancellationToken ct)
    {
        List<Paragraph> paragraphs = await _db.Paragraphs
            .AsNoTracking()
            .Where(r => r.PublicationId == chapter.PublicationId)
            .Where(r => r.Order >= chapter.Order)
            .Where(r => r.Order <= chapter.EndOrder)
            .OrderBy(r => r.Order)
            .ToListAsync(ct);
        return paragraphs;
    }

    /// <summary>
    /// Parses the paragraph content
    /// </summary>
    // ReSharper disable once CognitiveComplexity
    private string ParseParagraphContent(Publication publication, Paragraph paragraph, EpubChapter chapter)
    {
        var html = new HtmlDocument();
        // html.LoadHtml(HttpUtility.HtmlDecode(paragraph.Content));
        html.LoadHtml(paragraph.Content);
        html.OptionWriteEmptyNodes = true;
        if (paragraph.Content.Trim().StartsWith("<h") && paragraph.ParaId == chapter.ParaId)
        {
            html.DocumentNode.FirstChild.Attributes.Remove("class");
            html.DocumentNode.FirstChild.AddClass("chapterhead");
        }

        switch (publication.Type)
        {
            case PublicationType.Bible:
                switch (html.DocumentNode.FirstChild.Name.ToLower())
                {
                    case "h2":
                        html.DocumentNode.FirstChild.AddClass("bible-book-name");
                        _bibleBook = html.DocumentNode.FirstChild.InnerText;
                        break;
                    case "h3":
                        html.DocumentNode.FirstChild.InnerHtml =
                            $"{_bibleBook} {html.DocumentNode.FirstChild.InnerText}";
                        html.DocumentNode.FirstChild.AddClass("bible-book-number");
                        break;
                    case "h4":
                        html.DocumentNode.FirstChild.AddClass("bible-verse-number");
                        break;
                    case "p":
                        html.DocumentNode.FirstChild.AddClass("bible-verse-text");
                        break;
                }

                break;
            case PublicationType.Book:
                break;
            case PublicationType.Devotional:
                break;
            case PublicationType.BibleCommentary:
                break;
            case PublicationType.PeriodicalPageBreak:
                break;
            case PublicationType.PeriodicalNoPageBreak:
                break;
            case PublicationType.Manuscript:
                break;
            case PublicationType.Dictionary:
                break;
            case PublicationType.TopicalIndex:
                break;
            case PublicationType.ScriptureIndex:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        List<HtmlNode> pages = html.DocumentNode.QuerySelectorAll("span.page").ToList();
        if (pages.Any())
        {
            FormatPageTag(pages, chapter);
        }

        List<HtmlNode> footnotes = html.DocumentNode.QuerySelectorAll("sup").ToList();
        if (footnotes.Any(r => r.Attributes.AttributesWithName("class").Any()))
        {
            FormatFootnotes(footnotes, chapter);
        }

        return html.DocumentNode.WriteContentTo();
    }

    private void FormatFootnotes(List<HtmlNode> footnotes, EpubChapter chapter)
    {
        foreach (HtmlNode sup in footnotes)
        {
            List<string> clsList = sup.GetClasses().ToList();
            string? cls = clsList.FirstOrDefault(r => r.EndsWith("note"));
            if (cls is null)
            {
                continue;
            }

            {
                HtmlNode? span = sup.QuerySelector($"span.{cls}");
                if (span is not null)
                {
                    string innerHtml = string.Join("<br/>", span.QuerySelectorAll("p").Select(r => r.InnerHtml));

                    span.Remove();
                    var ft = new EpubFootNoteInfo
                    {
                        Id = _chapterFootnoteIdx, Class = cls, Header = sup.QuerySelector($"a.{cls}").InnerText
                    };
                    ft.Content =
                        $"""<p class="{cls}">[<a class="{cls}" id="fn{_chapterFootnoteIdx}" href="#ft{_chapterFootnoteIdx}">{ft.Header}</a>] {innerHtml.ReplaceLineEndings(" ").Replace("&nbsp;", " ")}</p>""";
                    sup.InnerHtml = $"<a class=\"{ft.Class}\" id=\"ft{ft.Id}\" href=\"#fn{ft.Id}\">{ft.Header}</a>";
                    chapter.Footnotes.Add(ft);
                }

                _chapterFootnoteIdx++;
            }
        }
    }

    private void FormatPageTag(List<HtmlNode> pages, EpubChapter chapter)
    {
        foreach (HtmlNode page in pages)
        {
            string? pageNumber = page.InnerText;
            page.Attributes.Remove("class");
            page.AddClass("pagebreak");
            page.SetAttributeValue("epub:type", "pagebreak");
            page.SetAttributeValue("id", $"p{pageNumber}");
            page.SetAttributeValue("title", pageNumber);
            chapter.Pages.Add(new EpubPageInfo
            {
                PageNumber = pageNumber, ContentSrc = $"{chapter.File}#p{pageNumber}", PlayOrder = -1
            });
            page.InnerHtml = $@" [{pageNumber}] ";
        }
    }
}