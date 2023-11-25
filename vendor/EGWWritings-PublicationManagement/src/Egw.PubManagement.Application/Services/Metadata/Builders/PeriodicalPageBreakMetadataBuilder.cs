using Egw.PubManagement.Application.Services.Metadata.Enrichers;
namespace Egw.PubManagement.Application.Services.Metadata.Builders;

/// <inheritdoc />
public class PeriodicalPageBreakMetadataBuilder : EnrichingMetadataBuilderBase
{
    /// <inheritdoc />
    protected override IEnumerable<IMetadataEnricher> GetEnricherList()
    {
        yield return new BookPaginationEnricher();
        yield return new PeriodicalPageBreakDateMetadataEnricher();
    }
}