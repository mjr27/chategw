using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <inheritdoc />
public class MoveFolderHandler : ApplicationCommandHandler<MoveFolderInput>
{
    /// <inheritdoc />
    public override async Task Handle(MoveFolderInput request, CancellationToken cancellationToken)
    {
        Folder sourceFolder = await _db.Folders
                                  .Include(r => r.Parent)
                                  .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
                              ?? throw new NotFoundProblemDetailsException("Folder not found");

        if (sourceFolder.ParentId == request.NewParent)
        {
            if (request.NewPosition.HasValue)
            {
                ReorderSameParent(sourceFolder, request.NewPosition.Value);
            }
        }
        else if (request.NewParent.HasValue == false)
        {
            throw new ConflictProblemDetailsException("Cant move folder to top level");
        }
        else
        {
            Folder targetFolder = await _db.Folders
                                      .FirstOrDefaultAsync(f => f.Id == request.NewParent, cancellationToken)
                                  ?? throw new NotFoundProblemDetailsException("Target folder not found");

            if (await _db.IsFolderContainsChild(request.Id, request.NewParent.Value, cancellationToken))
            {
                throw new ConflictProblemDetailsException("Target folder is child of source folder");
            }

            ReorderDifferentParent(sourceFolder, targetFolder, request.NewPosition);
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _db.RecalculateFolders(Now, cancellationToken);
    }

    private void ReorderSameParent(Folder folder, int position)
    {
        var children = _db.Folders
                .Where(r => r.ParentId == folder.ParentId)
                .Where(r => r.Id != folder.Id)
                .AsEnumerable()
                .Append(folder)
                .OrderBy(r => r.Order)
                .ToList();

        //fix the input to valid values
        if (position < 1)
            position = 1;
        else if (position > children.Count) 
            position = children.Count;

        //resorting all
        int step = 10;
        foreach (Folder child in children)
        {
            if (child.Id == folder.Id)
            {
                child.SetPosition(folder.ParentId, position * 10 - 5, Now);
            }
            else 
            { 
                child.SetPosition(child.ParentId, step, Now);
                step += 10;
            }
        }

        Normalize(children);
    }

    private void ReorderDifferentParent(
        Folder sourceFolder,
        Folder targetFolder,
        int? position)
    {
        Folder parentFolder = sourceFolder.Parent ?? throw new ConflictProblemDetailsException("Cannot move root folder");
        var parentChildren = _db.Folders.Where(r => r.ParentId == parentFolder.Id)
            .OrderBy(r => r.Order)
            .ToList();
        var targetChildren = _db.Folders.Where(r => r.ParentId == targetFolder.Id)
            .OrderBy(r => r.Order)
            .ToList();

        int oldPosition = sourceFolder.Order;
        foreach (Folder child in targetChildren.Where(child => child.Order >= position))
        {
            child.SetPosition(child.ParentId, child.Order + 1, Now);
        }

        foreach (Folder child in parentChildren.Where(child => child.Order > oldPosition))
        {
            child.SetPosition(child.ParentId, child.Order - 1, Now);
        }


        int newPosition = position
                          ?? (targetChildren.Any()
                              ? (targetChildren.Max(r => r.Order) + 1)
                              : 1);
        sourceFolder.SetPosition(targetFolder.Id, newPosition, Now);

        Normalize(parentChildren.Where(r => r.Id != sourceFolder.Id).ToList());
        Normalize(targetChildren.Append(sourceFolder).ToList());
    }

    private void Normalize(List<Folder> folders)
    {
        folders.Sort((a, b) => a.Order.CompareTo(b.Order));
        for (int i = 0; i < folders.Count; i++)
        {
            if (folders[i].Order != i + 1)
            {
                folders[i].SetPosition(folders[i].ParentId, i + 1, Now);
            }
        }
    }


    /// <inheritdoc />
    public MoveFolderHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}