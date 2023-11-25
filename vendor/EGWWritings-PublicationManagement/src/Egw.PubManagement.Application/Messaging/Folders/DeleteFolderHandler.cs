using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <inheritdoc />
public class DeleteFolderHandler : ApplicationCommandHandler<DeleteFolderInput>
{
    /// <inheritdoc />
    public override async Task Handle(DeleteFolderInput request, CancellationToken cancellationToken)
    {
        var foundFolder = await _db.Folders
                              .Select(r => new { r.Id, ChildFolders = r.Children.Count, ChildPublications = r.Placements.Count })
                              .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                          ?? throw new NotFoundProblemDetailsException("Folder not found");
        if (foundFolder.ChildFolders > 0)
        {
            throw new ConflictProblemDetailsException("You cannot delete a folder that has child folders");
        }

        if (foundFolder.ChildPublications > 0)
        {
            throw new ConflictProblemDetailsException("You cannot delete a folder that has publications");
        }

        Language? language = await _db.Languages
            .FirstOrDefaultAsync(r => r.RootFolderId == request.Id, cancellationToken);
        if (language is not null)
            language.SetRootFolderId(null, Now);

        Folder folder = await _db.Folders
            .SingleAsync(r => r.Id == request.Id, cancellationToken);
        _db.Folders.Remove(folder);
        await _db.SaveChangesAsync(cancellationToken);
        await _db.RecalculateFolders(Now, cancellationToken);
    }

    /// <inheritdoc />
    public DeleteFolderHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}