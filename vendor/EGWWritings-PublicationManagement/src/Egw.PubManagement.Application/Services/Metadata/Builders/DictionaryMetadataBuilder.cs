using Egw.PubManagement.Application.Services.Metadata.Enrichers;
namespace Egw.PubManagement.Application.Services.Metadata.Builders;

/// <inheritdoc />
public class DictionaryMetadataBuilder : EnrichingMetadataBuilderBase
{
    /// <inheritdoc />
    protected override IEnumerable<IMetadataEnricher> GetEnricherList()
    {
        yield return new DictionaryEnricher();
    }
}