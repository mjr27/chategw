using System.Text.RegularExpressions;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Serialization;
namespace Egw.PubManagement.LegacyImport.PublicationTypeEnrichers;

internal class PeriodicalEnricher : IPublicationTypeEnricher
{
    private const int PeriodicalPrefix = 2_000_000;

    private static readonly Regex EndsWithYear = new(@"\b\d{4}\b", RegexOptions.Compiled);

    public void EnrichDocument(WemlDocument wemlDocument, LegacyContext context)
    {
        foreach (WemlHeadingContainer? subHeader in wemlDocument.Children
                     .Select(r => r.Element)
                     .OfType<WemlHeadingContainer>()
                     .Where(r => r.Level > 1))
        {
            subHeader.Level++;
        }

        var serializer = new WemlSerializer();


        var titles = wemlDocument.Children
            .Where(el => el.IsHeaderOfLevel(3))
            .ToList();

        var years = new HashSet<int>();

        foreach (WemlParagraph? title in titles)
        {
            string titleText = title.Element.TextContent().Trim();
            if (!TryGetYear(titleText, out int year))
            {
                context.Report(
                    WarningLevel.Error,
                    serializer.Serialize(title.Element),
                    $"Cannot parse year from {titleText}"
                );
                continue; 
            }

            if (years.Contains(year))
            {
                continue;
            }

            years.Add(year);

            var element = new WemlParagraph(
                title.Id + PeriodicalPrefix,
                new WemlHeadingContainer(
                    2,
                    new WemlTextBlockElement(WemlParagraphType.Paragraph, new WemlTextNode(year.ToString()))
                )
            );
            wemlDocument.Children.Insert(wemlDocument.Children.IndexOf(title), element);
        }
    }

    private bool TryGetYear(string titleText, out int year)
    {
        year = 0;
        Match m = EndsWithYear.Match(titleText);
        return m.Success && int.TryParse(m.Value, out year);
    }
}