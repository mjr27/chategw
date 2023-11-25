using Egw.PubManagement.Persistence.Entities;
namespace Egw.PubManagement.Application.Services.Metadata;

/// <summary>
/// Metadata enricher
/// </summary>
public interface IMetadataEnricher
{
    /// <summary>
    /// Enriches paragraph metadata
    /// </summary>
    /// <param name="publication">Publication</param>
    /// <param name="paragraph">Current paragraph</param>
    /// <param name="metadata">Paragraph metadata</param>
    void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata);
}