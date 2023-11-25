using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.PublicationTypeEnrichers;

internal class TopicalIndexEnricher : IPublicationTypeEnricher
{
    private const int PrefixNumber = 2_000_000;
    private const int MaxItems = 800;
    private const string Mdash = "—";

    public void EnrichDocument(WemlDocument wemlDocument, LegacyContext context)
    {
        var letters = wemlDocument.Children
            .Where(el => el.IsHeaderOfLevel(2))
            .ToList();
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

            JoinGroup(letterContent, wemlDocument);
        }
    }

    private void JoinGroup(List<WemlParagraph> letterContent, WemlDocument wemlDocument)
    {
        var chunks = new List<List<WemlParagraph>>();
        foreach (WemlParagraph? paragraph in letterContent)
        {
            if (paragraph.IsHeaderOfLevel(4))
            {
                chunks.Add(new List<WemlParagraph> { paragraph });
            }
            else
            {
                chunks[^1].Add(paragraph);
            }
        }

        var buffer = new List<WemlParagraph>();
        foreach (List<WemlParagraph> chunk in chunks)
        {
            buffer.AddRange(chunk);

            if (buffer.Count <= MaxItems)
            {
                continue;
            }

            InsertGroup(buffer, wemlDocument);

            buffer.Clear();
        }

        if (buffer.Any())
        {
            InsertGroup(buffer, wemlDocument);
        }
    }

    private void InsertGroup(IReadOnlyList<WemlParagraph> buffer, WemlDocument wemlDocument)
    {
        WemlParagraph firstElement = buffer[0];
        WemlParagraph lastElement = buffer.Last(r => r.IsHeaderOfLevel(4));
        string startTitle = ExtractTitle(firstElement.Element);
        string endTitle = ExtractTitle(lastElement.Element);
        int position = wemlDocument.Children.IndexOf(firstElement);
        string title = $"{startTitle} {Mdash} {endTitle}";
        wemlDocument.Children.Insert(position,
            new WemlParagraph(
                PrefixNumber + firstElement.Id,
                new WemlHeadingContainer(
                    3,
                    new WemlTextBlockElement(WemlParagraphType.Paragraph, new WemlTextNode(title))
                )
            ));
    }

    private static string ExtractTitle(IWemlNode node)
    {
        IWemlNode rootElement = node.Descendants()
                                    .FirstOrDefault(r => r is WemlEntityElement { EntityType: WemlEntityType.Topic })
                                ?? node;
        return rootElement.TextContent().Trim();
    }
}