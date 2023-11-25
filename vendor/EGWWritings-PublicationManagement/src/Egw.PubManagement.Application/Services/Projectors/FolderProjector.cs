using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence.Entities;
namespace Egw.PubManagement.Application.Services.Projectors;

/// <inheritdoc />
public class FolderProjector : IEntityProjector<Folder, FolderDto>
{
    private static int[] SplitPath(string path)
    {
        return path
            .Split(Folder.MaterializedPathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
    }

    /// <inheritdoc />
    public Task<IQueryable<FolderDto>> Project(IQueryable<Folder> queryable, CancellationToken cancellationToken)
    {
        return Task.FromResult(queryable
            .OrderBy(r => r.GlobalOrder)
            .Select(r => new FolderDto
            {
                Id = r.Id,
                Path = SplitPath(r.MaterializedPath),
                Order = r.Order,
                Title = r.Title,
                TypeId = r.TypeId,
                UpdatedAt = r.UpdatedAt,
                ParentId = r.ParentId,
                CreatedAt = r.CreatedAt,
                ChildFolderCount = r.Children.Count,
                ChildPublicationCount = r.Placements.Count
            }));
    }
}