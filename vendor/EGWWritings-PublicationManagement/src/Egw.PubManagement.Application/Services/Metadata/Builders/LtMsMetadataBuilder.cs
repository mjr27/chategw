using System.Text.RegularExpressions;

using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Services.Metadata.Builders;

/// <inheritdoc />
public class LtMsMetadataBuilder : IMetadataBuilder
{
    private static readonly Regex ReLtMs = new(
        @"^(?<type>lt|ms)\s+(?<number>\d+)(?<letter>\w?)[,\s]\s*(?<year>\d{4}).*$",
        RegexOptions.IgnoreCase
    );

    private static readonly Regex ReSpace = new(@"\s+", RegexOptions.Multiline);

    /// <inheritdoc />
    public IEnumerable<ParagraphMetadata> GetMetadata(Publication publication,
        IEnumerable<Paragraph> paragraphs,
        DateTimeOffset currentDate)
    {
        List<List<Paragraph>> data = CombineParagraphs(paragraphs);

        foreach (List<Paragraph>? manuscript in data)
        {
            string ltMsName = manuscript.First().DeserializedContent().TextContent().Trim();
            Match match = ReLtMs.Match(ltMsName);
            if (!match.Success)
            {
                throw new ArgumentException($"Cannot parse lt/ms header : {ltMsName}");
            }

            bool isMs = match.Groups["type"].Value.ToUpperInvariant() == "MS";
            int number = int.Parse(match.Groups["number"].Value);
            char letter = string.IsNullOrEmpty(match.Groups["letter"].Value)
                ? '_'
                : match.Groups["letter"].Value.ToUpperInvariant()[0];
            int year = int.Parse(match.Groups["year"].Value);
            string ltMsCode = (isMs ? "M" : "L")
                              + number.ToString("0000")
                              + letter
                              + year.ToString("0000");
            int i = 1;

            IWemlContainerElement[] elements = manuscript.Select(r => r.DeserializedContent()).ToArray();

            string? addressee = GetFirstElement(elements, WemlParagraphRole.Addressee, WemlEntityType.Addressee);
            string? title = GetFirstElement(elements, WemlParagraphRole.Title);
            string? place = GetFirstElement(elements, WemlParagraphRole.Place, WemlEntityType.Place);
            DateOnly? date = FixDate(GetFirstElement(elements, WemlParagraphRole.Date, WemlEntityType.Date))
                             ?? new DateOnly(year, 1, 1);
            // FixDate(dateStr, year);

            foreach ((int idx, Paragraph para) in manuscript.Select((para, n) => (n, para)))
            {
                IWemlContainerElement[] el = { para.DeserializedContent() };
                title = GetFirstElement(el, WemlParagraphRole.Title) ?? title;
                place = GetFirstElement(el, WemlParagraphRole.Place, WemlEntityType.Place) ?? place;
                date = FixDate(GetFirstElement(el, WemlParagraphRole.Date, WemlEntityType.Date))
                       ?? date;
                var meta = new ParagraphMetadata(para.ParaId, currentDate);
                meta.WithDate(date, null); // TODO configure end date
                meta.WithManuscript(addressee, title, place);
                if (idx > 0 && para.IsReferenced)
                {
                    meta.WithPagination(ltMsCode, i++);
                }
                else
                {
                    meta.WithPagination(ltMsCode, 0);
                }

                yield return meta;
            }
        }
    }

    private static DateOnly? FixDate(string? dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr))
        {
            return null;
        }

        if (dateStr.All(char.IsDigit))
        {
            return new DateOnly(int.Parse(dateStr), 1, 1);
        }

        return DateOnly.TryParse(ReSpace.Replace(dateStr, " "), out DateOnly dateTmp)
            ? dateTmp
            : null;
    }


    private static string? FixSpaces(string? value) => value is null ? null : ReSpace.Replace(value.Trim(), " ");

    private string? GetFirstElement(
        IEnumerable<IWemlNode> nodes,
        WemlParagraphRole role,
        WemlEntityType? entityType = null)
    {
        return FixSpaces(nodes.OfType<WemlParagraphContainer>()
            .Where(r => r.Role == role)
            .Select(block =>
            {
                WemlEntityElement? firstEntity = block.DescendantsAndSelf()
                    .OfType<WemlEntityElement>()
                    .FirstOrDefault(r => r.EntityType == entityType);
                return firstEntity?.Value
                       ?? firstEntity?.TextContent()
                       ?? block.TextContent();
            })
            .FirstOrDefault());
    }

    private static List<List<Paragraph>> CombineParagraphs(IEnumerable<Paragraph> paragraphs)
    {
        var data = new List<List<Paragraph>>();

        foreach (Paragraph? para in paragraphs)
        {
            if (para is { HeadingLevel: > 0 and < 4 })
            {
                continue;
            }

            if (para.HeadingLevel == 4)
            {
                data.Add(new List<Paragraph>());
            }

            if (data.Count > 0)
            {
                data[^1].Add(para);
            }
        }

        return data;
    }
}