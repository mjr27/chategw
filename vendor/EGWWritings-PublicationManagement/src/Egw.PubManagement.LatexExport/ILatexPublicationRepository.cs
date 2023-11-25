using Egw.PubManagement.LatexExport.Models;

namespace Egw.PubManagement.LatexExport;

/// <summary>
/// Publication reader
/// </summary>
public interface ILatexPublicationRepository
{
    /// <summary>
    /// Retrieves publication data
    /// </summary>
    /// <param name="publicationId">Publication ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<LatexPublicationDto?> GetPublication(int publicationId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves paragraphs for publication
    /// </summary>
    /// <param name="publicationId">Publication ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    IAsyncEnumerable<LatexParagraphDto> GetParagraphs(int publicationId, CancellationToken cancellationToken);
}