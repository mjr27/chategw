using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using EnumsNET;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <inheritdoc />
public class ChangeFolderTypeHandler : ApplicationCommandHandler<ChangeFolderTypeInput>
{
    /// <inheritdoc />
    public ChangeFolderTypeHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }

    /// <inheritdoc />
    public override async Task Handle(ChangeFolderTypeInput request, CancellationToken cancellationToken)
    {
        Folder folder = await _db.Folders
                            .Include(r => r.Type)
                            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                        ?? throw new NotFoundProblemDetailsException("Folder not found");
        FolderType newType = await _db.FolderTypes
                                 .FirstOrDefaultAsync(r => r.FolderTypeId == request.Type, cancellationToken)
                             ?? throw new NotFoundProblemDetailsException("Folder type not found");
        List<PublicationType> childTypes = await _db.PublicationPlacement
            .Where(r => r.FolderId == request.Id)
            .Select(r => r.Publication.Type)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (
            PublicationType childType
            in childTypes
                .Where(childType => !newType.AllowedTypes.Contains(childType)))
        {
            throw new ValidationProblemDetailsException(
                $"Folder type {newType.FolderTypeId} does not allow publication type {childType.AsString(EnumFormat.Description)}");
        }

        folder.SetType(newType.FolderTypeId, Now);
        await _db.SaveChangesAsync(cancellationToken);
    }
}