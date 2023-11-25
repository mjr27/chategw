using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.PublicationTypeEnrichers;

internal class DictionaryEnricher : IPublicationTypeEnricher
{
    private const int DictionaryPrefix = 500000;
    private const int DictionarySectionSize = 101;
    private const string Mdash = "—";

    public void EnrichDocument(WemlDocument wemlDocument, LegacyContext context)
    {
        var letters = wemlDocument.Children
            .Where(el => el.IsHeaderOfLevel(2))
            .ToList();
        int maxItem = wemlDocument.Children.Select(r => r.Id).DefaultIfEmpty(0).Max();
        int dictionaryRangeChapterPrefix = maxItem > DictionaryPrefix
            ? 10_000_000
            : DictionaryPrefix;
        foreach (WemlParagraph? letter in letters)
        {
            var letterContent = wemlDocument.Children
                .SkipWhile(el => el.Id != letter.Id)
                .Skip(1)
                .TakeWhile(el => !el.IsHeaderOfLevel(2))
                .ToList();
            foreach (WemlHeadingContainer? item in letterContent.Select(r => r.Element).OfType<WemlHeadingContainer>())
            {
                item.Level++;
            }

            var chapters = letterContent
                .Where(el => el.IsHeaderOfLevel(4))
                .ToList();
            for (int i = 0; i < chapters.Count; i += DictionarySectionSize)
            {
                WemlParagraph currentChapter = chapters[i];
                WemlParagraph lastChapter = chapters.Skip(i).Take(DictionarySectionSize).Last();
                string title = ExtractTitle(currentChapter.Element) + " " + Mdash + " " +
                               ExtractTitle(lastChapter.Element);
                int idx = wemlDocument.Children.IndexOf(currentChapter);
                wemlDocument.Children.Insert(idx, new WemlParagraph(
                    dictionaryRangeChapterPrefix + currentChapter.Id,
                    new WemlHeadingContainer(3,
                        new WemlTextBlockElement(WemlParagraphType.Paragraph, new WemlTextNode(title))
                    )
                ));
            }
        }
    }

    private static string ExtractTitle(IWemlNode node)
    {
        IWemlNode rootElement = node.Descendants()
                                    .FirstOrDefault(r => r is WemlEntityElement { EntityType: WemlEntityType.Topic })
                                ?? node;
        return rootElement.TextContent().Trim();
    }
}