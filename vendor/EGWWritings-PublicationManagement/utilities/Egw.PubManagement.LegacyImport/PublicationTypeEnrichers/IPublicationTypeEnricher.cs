using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.PublicationTypeEnrichers;

/// <summary>
/// Publication type enricher
/// </summary>
internal interface IPublicationTypeEnricher
{
    /// <summary>
    /// Enriches given document
    /// </summary>
    /// <param name="document"></param>
    /// <param name="context"></param>
    void EnrichDocument(WemlDocument document, LegacyContext context);
}