using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <inheritdoc />
public class RenameFolderHandler : IRequestHandler<RenameFolderInput>
{
    private readonly PublicationDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// Default constructor
    /// </summary>
    public RenameFolderHandler(PublicationDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task Handle(RenameFolderInput request, CancellationToken cancellationToken)
    {
        Folder folder = await _db.Folders.FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
                        ?? throw new KeyNotFoundException($"Folder with id {request.Id} not found");
        folder.SetTitle(request.Title, _clock.Now);
        await _db.SaveChangesAsync(cancellationToken);
    }
}