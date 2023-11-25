using System.Data;
using System.Net;
using System.Web;

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using Dapper;

using INode = AngleSharp.Dom.INode;

namespace Egw.ImportUtils.Services;

/// <summary>
/// Book exporter
/// </summary>
internal class BookExporter
{
    private readonly HtmlParser _parser;

    /// <summary>
    /// Book exporter
    /// </summary>
    public BookExporter()
    {
        _parser = new HtmlParser();
    }

    /// <summary>
    /// Exports publication from legacy db
    /// </summary>
    /// <param name="connection">Legacy DB Connection</param>
    /// <param name="publicationId">Publication id</param>
    /// <param name="writer">Text Writer</param>
    public async Task ExportPublication(IDbConnection connection, int publicationId, StreamWriter writer)
    {
        IHtmlDocument doc = CreateDocument();
        dynamic? book = await connection.QuerySingleOrDefaultAsync(PublicationQuery, new { bookId = publicationId });
        if (book is null)
        {
            throw new ArgumentException("Publication not found");
        }

        int punctuationId = book.PunctuationId;
        var punctuation = connection.Query(PunctuationQuery, new { scheme = punctuationId })
            .ToDictionary(r => (string)r.entity, r => (string)r.punct);
        doc.DocumentElement.SetAttribute("lang", book.Language);

        IHtmlHeadElement head = doc.Head ?? throw new Exception("No head");
        head.QuerySelector("title")!.TextContent = WebUtility.HtmlDecode(book.PubName);
        head.AppendChild(doc.CreateComment($" id pub: {book.IdPub} "));
        IHtmlTableElement table = doc.CreateElement<IHtmlTableElement>();
        table.ClassName = "pubinfotable";
        table.Border = 1;

        IHtmlTableRowElement row = doc.CreateElement<IHtmlTableRowElement>();
        IHtmlTableHeaderCellElement th1 = doc.CreateElement<IHtmlTableHeaderCellElement>();
        th1.TextContent = "SQL FieldName";
        IHtmlTableHeaderCellElement th2 = doc.CreateElement<IHtmlTableHeaderCellElement>();
        th2.TextContent = "Value";
        row.AppendNodes(th1, th2);

        table.AppendChild(row);

        AddTableRow("pubnr", publicationId.ToString(), table, doc);
        AddTableRow("lang", book.Language, table, doc);
        AddTableRow("publicationcode", book.PubCode, table, doc);
        AddTableRow("publicationname", book.PubName, table, doc);
        AddTableRow("pubtype", book.PubType, table, doc);
        AddTableRow("subtype", book.SubType, table, doc);

        IHtmlElement body = doc.Body ?? throw new Exception("No body");
        const string marker = "XXXZZZXXX";
        body.AppendElement(table);
        body.Append(doc.CreateTextNode(marker));

        string html = doc.ToHtml(new PrettyMarkupFormatter());
        int markerIdx = html.IndexOf(marker, StringComparison.Ordinal);
        string header = html[..markerIdx];
        string footer = html[(markerIdx + marker.Length)..];
        await writer.WriteAsync(header.Trim() + "\n");

        IEnumerable<(int IdElement, string Element, string Property1, string Property2, string? ContentText)>
            paragraphs =
                connection
                    .Query<(int IdElement, string Element, string Property1, string Property2, string? ContentText)>(
                        ParagraphsQuery,
                        new { bookId = publicationId });

        foreach ((int IdElement, string Element, string Property1, string Property2, string? ContentText) paragraph in
                 paragraphs)
        {
            string text = paragraph.ContentText ?? "";

            foreach ((string entity, string punct) in punctuation)
            {
                text = text.Replace(entity, punct);
            }

            IElement element = doc.CreateElement(paragraph.Element == "p" ? "div" : paragraph.Element);

            element.SetAttribute("id", book.PubCode + "." + paragraph.IdElement);


            string className = $"{paragraph.Property1} {paragraph.Property2}".Trim();
            if (className != "")
            {
                element.ClassName = className;
            }

            element.InnerHtml = text.Trim();
            await writer.WriteAsync(
                "\t\t" + element.OuterHtml + "\n"
            );
        }

        await writer.WriteAsync('\t');
        await writer.WriteAsync(footer);

        // await doc.ToHtmlAsync(writer);
    }

    private static void AddTableRow(string title, string value, INode table, IDocument document)
    {
        IHtmlTableRowElement row = document.CreateElement<IHtmlTableRowElement>();
        IHtmlTableCellElement titleCell = document.CreateElement<IHtmlTableCellElement>();
        titleCell.TextContent = HttpUtility.HtmlDecode(title);
        row.AppendChild(titleCell);
        IHtmlTableCellElement valueCell = document.CreateElement<IHtmlTableCellElement>();
        valueCell.TextContent = HttpUtility.HtmlDecode(value);
        row.AppendChild(valueCell);
        table.AppendChild(row);
    }


    private IHtmlDocument CreateDocument()
    {
        return _parser.ParseDocument(HtmlTemplate);
    }

    private const string HtmlTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<title></title>
</head>
<body>
</body>
</html>
";

    private const string PublicationQuery = @"
    -- noinspection SqlResolve
    select
                    pubnr as BookId,
                    id_pub as IdPub,
                    pubtype as PubType,
                    subtype as SubType,
                    el.lang_country as Language,
                    el.punctuation as PunctuationId,
                    publicationcode as PubCode,
                    publicationname as PubName,
                    publisher       as Publisher
                from sqlb_publicationoverview
                left outer join egw_language el on sqlb_publicationoverview.language0 = el.name
                where actversion = 1
                    AND pubnr=@bookId
    ";


    private const string ParagraphsQuery = @"
    -- noinspection SqlResolve
    select p.id_element  as IdElement,
           p.element     as Element,
           p.property1   as Property1,
           p.property2   as Property2,
           p.contenttext as ContentText
    from sqlb_publications p
             inner join sqlb_publicationoverview po on p.id_pub = po.id_pub
    where po.pubnr = @bookId
      and po.actversion
      and (p.property3 = 'infolio' or p.property1 = 'page' or p.property2 ='page' or p.contenttext like '%span class=""page%')
        and not p.property1 = 'toc'
        order by p.puborder";

    private const string PunctuationQuery = @"
-- noinspection SqlResolve
select entity, punct
from sqlb_punctuation
where scheme = @scheme;
";
}