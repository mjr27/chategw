using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Services.Metadata.Enrichers;

internal class PeriodicalPageBreakDateMetadataEnricher : IMetadataEnricher
{
    private DateOnly _currentDate;

    public PeriodicalPageBreakDateMetadataEnricher()
    {
        _currentDate = DateOnly.MinValue;
    }

    public void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata)
    {
        switch (paragraph.HeadingLevel)
        {
            case 3:
                {
                    IWemlContainerElement node = paragraph.DeserializedContent();
                    string dateFormat = node.TextContent();
                    if (!DateParser.TryParseDate(dateFormat, out DateOnly date))
                    {
                        throw new FormatException($"Invalid format {dateFormat} in {paragraph.ParaId}");
                    }

                    _currentDate = date;
                    metadata.WithDate(_currentDate, null); // TODO configure end date
                    break;
                }
            case 0 or > 3 or null:
                {
                    if (_currentDate == DateOnly.MinValue)
                    {
                        return;
                    }

                    metadata.WithDate(_currentDate, null); // TODO configure end date
                    break;
                }
        }
    }
}