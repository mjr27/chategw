using Egw.PubManagement.Application.Services;
using Egw.PubManagement.LatexExport;
using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Services.Latex;

/// <summary>
/// Cover repository1
/// </summary>
public class DatabaseLatexCoverRepository : ILatexCoverRepository
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IStorageWrapper _storageWrapper;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="dbContextFactory">Publication database context factory</param>
    /// <param name="storageWrapper">Storage wrapper</param>
    public DatabaseLatexCoverRepository(IDbContextFactory<PublicationDbContext> dbContextFactory,
        IStorageWrapper storageWrapper)
    {
        _dbContextFactory = dbContextFactory;
        _storageWrapper = storageWrapper;
    }

    /// <inheritdoc />
    public async Task<byte[]?> ReadCover(int publicationId, CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        Uri? coverUri = await db.Covers
            .Where(r => r.TypeId == "web" && r.IsMain && r.PublicationId == publicationId)
            .Select(r => r.Uri)
            .FirstOrDefaultAsync(cancellationToken);
        if (coverUri is null)
        {
            return null;
        }

        using var ms = new MemoryStream();
        await using Stream? stream = await _storageWrapper.Covers.Read(coverUri.ToString(), cancellationToken);
        if (stream is null)
        {
            return null;
        }

        await stream.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }
}