using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;

using HotChocolate.Data;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Models;

/// <summary>
/// DTO For <see cref="FolderType"/>
/// </summary>
public class FolderTypeDto : IProjectedDto<FolderType>
{
    /// <summary>
    /// Folder type Id
    /// </summary>
    [IsProjected]
    public string Id { get; init; } = "";

    /// <summary>
    /// Folder type name
    /// </summary>
    public string Title { get; init; } = "";

    /// <summary>
    /// Allowed publication types
    /// </summary>
    public PublicationType[] AllowedTypes { get; init; } = Array.Empty<PublicationType>();

    /// <summary>
    /// Created at
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Updated at
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}