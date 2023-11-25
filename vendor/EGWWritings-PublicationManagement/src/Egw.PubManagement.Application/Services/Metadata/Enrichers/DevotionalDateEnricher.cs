using Egw.PubManagement.Persistence.Entities;
namespace Egw.PubManagement.Application.Services.Metadata.Enrichers;

internal class DevotionalDateEnricher : IMetadataEnricher
{
    private int _month;
    private int _day;

    public DevotionalDateEnricher()
    {
        _month = 0;
        _day = 0;
    }

    public void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata)
    {
        if (paragraph.HeadingLevel == 0 && _day > 0 && _month > 0)
        {
            try
            {
                metadata.WithDate(new DateOnly(2000, _month, _day), null);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(
                    $"Invalid date: 2000-{_month}-{_day} for paragraph {paragraph.ParaId}");
            }

            return;
        }

        if (!paragraph.IsReferenced)
        {
            return;
        }

        switch (paragraph.HeadingLevel)
        {
            case 2:
                _month++;
                _day = 0;
                break;
            case 3:
                _day++;
                try
                {
                    metadata.WithDate(new DateOnly(2000, _month, _day), null);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Invalid date: {0}-{1}-{2} for paragraph {3}", 2000, _month, _day,
                        paragraph.ParaId);
                    throw;
                }

                break;
        }
    }
}