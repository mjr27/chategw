using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using HotChocolate.Data;

namespace Egw.PubManagement.Application.Models;

/// <summary>
/// Cover DTO
/// </summary>
public class CoverDto : IProjectedDto<Cover>
{
    /// <summary> Cover ID </summary>
    [IsProjected]
    public required Guid Id { get; init; }

    /// <summary> Is main cover for a publication/type </summary>
    public bool IsMain { get; set; }

    /// <summary>  Cover type code </summary>
    [IsProjected]
    public required string TypeId { get; init; }

    /// <summary> Publication identifier </summary>
    public required int PublicationId { get; init; }

    /// <summary> Original width </summary>
    public required int Width { get; init; }

    /// <summary> Original height </summary>
    public required int Height { get; init; }

    /// <summary> Original file size </summary>
    public required long Size { get; init; }

    /// <summary> Media format </summary>
    public required MediaFormatEnum Format { get; init; }

    /// <summary> Image URI </summary>
    [IsProjected]
    public required Uri Uri { get; init; }

    /// <summary>
    /// Created at
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}