using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Extensions;

/// <summary>
/// Database extensions
/// </summary>
public static class DbExtensions
{
    /// <summary>
    /// Returns the default heading level for a publication type
    /// </summary>
    /// <param name="publicationType">Publication type</param>
    /// <returns>Heading level</returns>
    public static int GetDefaultHeadingLevel(PublicationType publicationType)
    {
        return publicationType switch
        {
            PublicationType.Manuscript => 4,
            PublicationType.Bible => 3,
            PublicationType.PeriodicalPageBreak or PublicationType.PeriodicalNoPageBreak => 4,
            PublicationType.Dictionary => 3,
            PublicationType.TopicalIndex => 3,
            PublicationType.ScriptureIndex => 3,
            PublicationType.BibleCommentary => 4,
            _ => 4
        };
    }

    /// <summary>
    /// Recalculates folder position in the database
    /// </summary>
    /// <param name="db"></param>
    /// <param name="moment"></param>
    /// <param name="cancellationToken"></param>
    public static async Task RecalculateFolders(this PublicationDbContext db,
        DateTimeOffset moment,
        CancellationToken cancellationToken)
    {
        List<Folder> folders = await db.Folders.ToListAsync(cancellationToken);
        List<Folder> targetFolders = RecalculateFolders(null, folders, moment);
        for (int i = 0; i < targetFolders.Count; i++)
        {
            targetFolders[i].SetGlobalPosition(i);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static List<Folder> RecalculateFolders(Folder? parent, IReadOnlyCollection<Folder> folders, DateTimeOffset moment)
    {
        string parentPath = parent?.MaterializedPath is null
            ? ""
            : parent.MaterializedPath + Folder.MaterializedPathSeparator;
        string format = new('0', Folder.MaterializedPathElementLength);

        var result = new List<Folder>();
        var childFolders = folders.Where(r => r.ParentId == parent?.Id)
            .OrderBy(r => r.Order)
            .ToList();
        for (int i = 0; i < childFolders.Count; i++)
        {
            Folder f = childFolders[i];
            f.SetPosition(f.ParentId, i + 1, moment);
            f.MaterializedPath = parentPath + f.Id.ToString(format);
            List<Folder> children = RecalculateFolders(f, folders, moment);
            result.Add(f);
            result.AddRange(children);
        }

        return result;
    }

    internal static async Task<bool> IsFolderContainsChild(this PublicationDbContext db, int folderId, int childId, CancellationToken cancellationToken)
    {
        bool result = false;

        List<Folder> childFolders = await db.Folders.Where(r => r.ParentId == folderId).ToListAsync(cancellationToken);
        for (int i = 0; i < childFolders.Count; i++)
        {
            if (childFolders[i].Id == childId)
                return true;

            result = await IsFolderContainsChild(db, childFolders[i].Id, childId, cancellationToken);
        }

        return result;
    }

    internal static async Task FixFolderOrdering(this PublicationDbContext db,
        int folderId,
        DateTimeOffset moment,
        CancellationToken cancellationToken)
    {
        PublicationPlacement[] placements = db.PublicationPlacement.Where(r => r.FolderId == folderId)
            .OrderBy(r => r.Order).ToArray();
        for (int i = 0; i < placements.Length; i++)
        {
            PublicationPlacement row = placements[i];
            row.ChangePlacement(row.FolderId, i + 1, moment);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    internal static async Task<bool> AuthorExist(this PublicationDbContext db,
        int? authorId,
        CancellationToken cancellationToken)
    {
        return await db.Authors.Where(r => r.Id == authorId).AnyAsync(cancellationToken);
    }

    internal static async Task<Folder?> GetRootFolder(this PublicationDbContext db, int? folderId, CancellationToken cancellationToken)
    {
        Folder? folder = await db.Folders.FirstOrDefaultAsync(r => r.Id == folderId, cancellationToken);

        if (folder is not null && folder.ParentId is not null)
        {
            return await GetRootFolder(db, folder.ParentId, cancellationToken);
        }
        else
        {
            return folder;
        }
    }
    internal static async Task FixParagraphOrdering(this PublicationDbContext db,
        int publicationId,
        DateTimeOffset moment,
        CancellationToken cancellationToken)
    {
        Paragraph[] paragraphs = db.Paragraphs.Where(r => r.PublicationId == publicationId)
            .OrderBy(r => r.Order).ToArray();
        for (int i = 0; i < paragraphs.Length; i++)
        {
            Paragraph row = paragraphs[i];
            row.ChangeOrder(i + 1, moment);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}