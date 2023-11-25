using System.Diagnostics;

using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Application.Messaging.Publications;

/// <inheritdoc />
public class MovePublicationHandler : ApplicationCommandHandler<MovePublicationInput>
{
    /// <inheritdoc />
    public MovePublicationHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }

    /// <inheritdoc />
    public override async Task Handle(
        MovePublicationInput request,
        CancellationToken cancellationToken
    )
    {
        Publication publication = await _db.Publications
                                      .FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId,
                                          cancellationToken)
                                  ?? throw new NotFoundProblemDetailsException("Publication not found");

        Folder folder = await _db.Folders
                            .FirstOrDefaultAsync(r => r.Id == request.FolderId, cancellationToken)
                        ?? throw new NotFoundProblemDetailsException("Folder not found");

        //Publication may only be moved to a leaf folder (folder without any subfolders)
        bool folderWithChildren = await _db.Folders.Where(r => r.ParentId == folder.Id).AnyAsync(cancellationToken);
        if (folderWithChildren)
        {
            throw new ConflictProblemDetailsException("Folder contains subfolders");
        }

        //Publication may only be moved to a folder within the same language
        Folder rootFolder = await _db.GetRootFolder(folder.Id, cancellationToken)
                            ?? throw new ConflictProblemDetailsException("Root folder not found");

        Language language = await _db.Languages
                                .FirstOrDefaultAsync(r => r.RootFolderId == rootFolder.Id, cancellationToken)
                            ?? throw new NotFoundProblemDetailsException("Language for root folder not found");

        if (language.Code != publication.LanguageCode)
        {
            throw new ConflictProblemDetailsException("Wrong language for publication");
        }

        //Publication may only be moved to a folder with a corresponding type
        FolderType folderType = await _db.FolderTypes
                                    .FirstOrDefaultAsync(r => r.FolderTypeId == folder.TypeId, cancellationToken)
                                ?? throw new NotFoundProblemDetailsException("Folder type not found");
        if (folderType.AllowedTypes.Contains(publication.Type) == false)
        {
            throw new ConflictProblemDetailsException("Publication type not allowed for this folder");
        }

        //Current placement
        PublicationPlacement placement = await _db.PublicationPlacement
                                             .FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId,
                                                 cancellationToken)
                                         ?? throw new NotFoundProblemDetailsException("Placement not found");

        int oldFolderId = placement.FolderId;

        if (request.Order is null)
        {
            placement.ChangePlacement(request.FolderId, int.MaxValue, Now);
        }
        else
        {
            await ChangePlacement(request, placement, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _db.FixFolderOrdering(request.FolderId, Now, cancellationToken);

        if (oldFolderId != request.FolderId)
        {
            await _db.FixFolderOrdering(oldFolderId, Now, cancellationToken);
        }
    }

    private async Task ChangePlacement(MovePublicationInput request,
        PublicationPlacement placement,
        CancellationToken cancellationToken)
    {
        Debug.Assert(request.Order is not null, "request.Order is not null");
        PublicationPlacement[] placements = await _db.PublicationPlacement
            .Where(r => r.FolderId == request.FolderId && r.PublicationId != request.PublicationId)
            .OrderBy(r => r.Order)
            .ToArrayAsync(cancellationToken);

        foreach (PublicationPlacement placementBelow in placements)
        {
            if (placementBelow.Order > request.Order)
                placementBelow.ChangePlacement(request.FolderId, placementBelow.Order + 1 + 1000, Now);
            else
                placementBelow.ChangePlacement(request.FolderId, placementBelow.Order - 1 + 1000, Now);
        }

        placement.ChangePlacement(request.FolderId, (int)request.Order + 1000, Now);
    }
}