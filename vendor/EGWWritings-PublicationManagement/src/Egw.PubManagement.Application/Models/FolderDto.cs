using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;

using HotChocolate.Data;
namespace Egw.PubManagement.Application.Models;

/// <summary>
/// DTO for <see cref="Folder"/>
/// </summary>
public sealed class FolderDto : IProjectedDto<Folder>
{
    /// <summary>
    /// Unique folder id
    /// </summary>
    [IsProjected]
    public int Id { get; init; }

    /// <summary>
    /// Folder parent ID
    /// </summary>
    [IsProjected]
    public int? ParentId { get; init; }

    /// <summary>
    /// Order
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; init; } = "";

    /// <summary>
    /// Folder type ID
    /// </summary>
    [IsProjected]
    public string TypeId { get; init; } = "";
    /// <summary>
    /// Folder type ID
    /// </summary>
    [IsProjected]
    public int[] Path { get; set; } = Array.Empty<int>();

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Date of last modification
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }

    /// <summary>
    /// Child folder count
    /// </summary>
    public int ChildFolderCount { get; init; }
    /// <summary>
    /// Child publication count
    /// </summary>
    public int ChildPublicationCount { get; init; }
}