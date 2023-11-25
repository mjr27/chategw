using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Unpublishes a publication
/// </summary>
public class PublishPublicationHandler : ApplicationCommandHandler<PublishPublicationInput>
{
    /// <inheritdoc />
    public override async Task Handle(PublishPublicationInput request, CancellationToken cancellationToken)
    {
        PublicationPlacement? placement = await _db.PublicationPlacement
            .Where(r => r.PublicationId == request.PublicationId)
            .FirstOrDefaultAsync(cancellationToken);
        if (placement is not null)
        {
            throw new ConflictProblemDetailsException("Publication is already published");
        }

        Publication? publication = await _db.Publications
            .FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId, cancellationToken);
        if (publication is null)
        {
            throw new NotFoundProblemDetailsException("Publication not found");
        }

        Folder? folder = await _db.Folders
            .Include(r => r.Type)
            .FirstOrDefaultAsync(r => r.Id == request.FolderId, cancellationToken);
        if (folder is null)
        {
            throw new NotFoundProblemDetailsException("Folder not found");
        }

        int childFolderCount = await _db.Folders
            .Where(r => r.ParentId == request.FolderId)
            .CountAsync(cancellationToken);

        if (childFolderCount > 0)
        {
            throw new ConflictProblemDetailsException("Cannot publish to a folder with child folders");
        }

        placement = new PublicationPlacement(request.PublicationId, request.FolderId, int.MaxValue, Now);
        placement.ChangePermission(PublicationPermissionEnum.Hidden, Now);
        _db.PublicationPlacement.Add(placement);
        await _db.SaveChangesAsync(cancellationToken);
        await _db.FixFolderOrdering(request.FolderId, Now, cancellationToken);
    }

    /// <inheritdoc />
    public PublishPublicationHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }


}