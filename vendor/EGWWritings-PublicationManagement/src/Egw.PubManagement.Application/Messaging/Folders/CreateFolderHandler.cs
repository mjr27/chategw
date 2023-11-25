using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <inheritdoc />
public class CreateFolderHandler : ApplicationCommandHandler<CreateFolderInput>
{
    /// <inheritdoc />
    public override async Task Handle(CreateFolderInput request, CancellationToken cancellationToken)
    {
        if (!await _db.Folders.AnyAsync(r => r.Id == request.ParentId, cancellationToken))
        {
            throw new NotFoundProblemDetailsException($"Folder with ID {request.ParentId} not found");
        }

        // Creates a new folder at the end of specified parent folder
        int maxOrder = _db.Folders
            .Where(r => r.ParentId == request.ParentId)
            .Select(r => r.Order)
            .DefaultIfEmpty()
            .Max();

        var folder = new Folder(request.Title, request.ParentId, maxOrder + 1, request.Type, Now);
        _db.Folders.Add(folder);
        await _db.SaveChangesAsync(cancellationToken);
        await _db.RecalculateFolders(Now, cancellationToken);
    }

    /// <inheritdoc />
    public CreateFolderHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}