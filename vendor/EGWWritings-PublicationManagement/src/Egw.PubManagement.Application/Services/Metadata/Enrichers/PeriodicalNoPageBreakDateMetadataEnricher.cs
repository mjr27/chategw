using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Services.Metadata.Enrichers;

internal class PeriodicalNoPageBreakDateMetadataEnricher : IMetadataEnricher
{
    private DateOnly _currentDate;
    private char _currentArticle;
    private int _paragraphNumber;

    public PeriodicalNoPageBreakDateMetadataEnricher()
    {
        _currentDate = DateOnly.MinValue;
        _currentArticle = 'A';
        _paragraphNumber = 0;
    }

    public void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata)
    {
        if (paragraph.HeadingLevel == 3)
        {
            ProcessDateHeading(paragraph);
            metadata.WithDate(_currentDate, null); // TODO configure end date
            metadata.WithPagination(_currentArticle.ToString(), 0);
            return;
        }

        if (paragraph.HeadingLevel is null or 1 or 2)
        {
            return;
        }

        if (_currentDate == DateOnly.MinValue)
        {
            return;
        }

        metadata.WithDate(_currentDate, null); // TODO configure end date
        metadata.WithPagination(_currentArticle.ToString(),
            paragraph is { IsReferenced: true, HeadingLevel: 0 } ? _paragraphNumber++ : 0);
    }

    private void ProcessDateHeading(Paragraph paragraph)
    {
        IWemlContainerElement node = paragraph.DeserializedContent();
        string dateFormat = node.TextContent().Trim();
        if (!DateParser.TryParseDate(dateFormat, out DateOnly date))
        {
            throw new FormatException($"Invalid format {dateFormat} in {paragraph.ParaId}");
        }

        if (_currentDate == date)
        {
            _currentArticle++;
        }
        else
        {
            _currentArticle = 'A';
            _currentDate = date;
        }

        _paragraphNumber = 1;
    }
}