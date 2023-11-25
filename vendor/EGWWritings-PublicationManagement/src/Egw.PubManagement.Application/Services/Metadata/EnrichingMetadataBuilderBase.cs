using Egw.PubManagement.Persistence.Entities;
namespace Egw.PubManagement.Application.Services.Metadata;

/// <inheritdoc />
public abstract class EnrichingMetadataBuilderBase : IMetadataBuilder
{
    /// <summary>
    /// Returns list of paragraph metadata enrichers
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<IMetadataEnricher> GetEnricherList();

    /// <inheritdoc />
    public IEnumerable<ParagraphMetadata> GetMetadata(
        Publication publication,
        IEnumerable<Paragraph> paragraphs, DateTimeOffset currentDate
    )
    {
        IMetadataEnricher[] enrichers = GetEnricherList().ToArray();
        foreach (Paragraph? para in paragraphs)
        {
            var metadata = new ParagraphMetadata(para.ParaId, currentDate);
            foreach (IMetadataEnricher? enricher in enrichers)
            {
                enricher.EnrichMetadata(publication, para, metadata);
            }

            yield return metadata;
        }
    }
}