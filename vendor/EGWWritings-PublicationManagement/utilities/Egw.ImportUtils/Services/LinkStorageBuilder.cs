using System.Data;
using System.Runtime.CompilerServices;

using Dapper;

using Egw.ImportUtils.Services.Cache;
using Egw.PubManagement.Application.Models.Internal;
using Egw.PubManagement.LegacyImport.BibleCodes;

using ShellProgressBar;

namespace Egw.ImportUtils.Services;

internal class LinkStorageBuilder
{
    private readonly LinkStorage _db;

    public LinkStorageBuilder(LinkStorage db)
    {
        _db = db;
    }

    public void CollectLanguages(IDbConnection connection)
    {
        IEnumerable<(string, string)> languages =
            connection.Query<(string, string)>(
                "SELECT DISTINCT lang_country, marccode FROM egw_language ORDER BY marccode");
        _db.Languages.Clear();

        foreach ((string fullName, string shortName) in languages.Where(r =>
                     !string.IsNullOrWhiteSpace(r.Item1) && !string.IsNullOrWhiteSpace(r.Item2)))
        {
            _db.AddLanguage(shortName, fullName);
        }

        _db.AddLanguage("kjv", "en-us");
        _db.AddLanguage("rccv", "ro-ro");
        _db.AddLanguage("tag", "tl-ph");
    }


    public void CollectLinks(IDbConnection connection, IReadOnlyCollection<int> publications)
    {
        var books = connection.Query<LinkGeneratorBookDto>(BooksQuery, new { books = publications }).ToList();

        var generator = new LinkGenerator(new BibleLinkNormalizer());

        var rawData = new Dictionary<(string lang, string code), string>();


        using (var progress = new ProgressBar(books.Count, "Collecting links"))
        {
            foreach (LinkGeneratorBookDto? book in books)
            {
                progress.Tick();
                IEnumerable<LinkGeneratorParagraph> paragraphs = connection
                    .Query<LinkGeneratorParagraph>(ParagraphsQuery, new { book.IdPub });

                foreach (LinkGeneratorParagraphLinks paragraph in generator.GenerateLinksFor(book, paragraphs))
                {
                    AppendToData(rawData, paragraph);
                }
            }
        }

        _db.Links.Clear();
        var data = new Dictionary<string, Dictionary<string, string>>();
        foreach (((string lang, string code) key, string reference) in rawData)
        {
            if (!data.TryGetValue(key.lang, out Dictionary<string, string>? langDict))
            {
                langDict = new Dictionary<string, string>();
                data[key.lang] = langDict;
            }

            string code = key.code.ToLowerInvariant();
            langDict.TryAdd(code, reference);
        }

        foreach ((string lang, string code, string reference) in data
                     .SelectMany(r => r.Value.Select(r2 => (lang: r.Key, code: r2.Key, reference: r2.Value))))
        {
            _db.AddLink(lang, code, reference);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendToData(IDictionary<(string lang, string code), string> data,
        LinkGeneratorParagraphLinks linkGeneratorParagraph)
    {
        foreach (string refCode in linkGeneratorParagraph.RefCodes)
        {
            (string lang, string code) key = (linkGeneratorParagraph.LanguageCode, refCode);
            if (!data.ContainsKey(key))
            {
                data[key] = $"{linkGeneratorParagraph.BookId}.{linkGeneratorParagraph.ParagraphId}";
            }
        }
    }

    private const string BooksQuery = @"select
                pubnr as BookId,
                id_pub as IdPub,
                pubtype as PubType,
                subtype as SubType,
                el.lang_country as Language,
                publicationcode as PubCode,
                publicationname as PubName,
                publisher       as Publisher
            from sqlb_publicationoverview
            left outer join egw_language el on sqlb_publicationoverview.language0 = el.name
            where actversion = 1
            AND pubnr IN @books
            order by pubnr";

    private const string ParagraphsQuery = @"-- noinspection SqlDialectInspection
    select id_element as IdElement,
               IF(property2 = 'countpara-partofprevious', 'true', 'false') as IsContinuation,
               element as Element,
               property1 as Property1,
               refcode2 as Ref2,
               refcode3 as Ref3,
               refcode4 as Ref4,
               refcode5 as Ref5,
               refcode6 as Ref6,
               refcode7 as Ref7,
               contenttext as Content
        from sqlb_publications
        where id_pub = @idPub
          and property3 = 'infolio'
          and property1 != 'toc'
        order by puborder
    ";
}