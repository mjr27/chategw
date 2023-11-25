namespace Egw.PubManagement.LatexExport;

/// <summary>
/// Cover
/// </summary>
public interface ILatexCoverRepository
{
    /// <summary>
    /// Reads cover image
    /// </summary>
    /// <param name="publicationId">Publication id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Contents of the cover image</returns>
    Task<byte[]?> ReadCover(int publicationId, CancellationToken cancellationToken);
}