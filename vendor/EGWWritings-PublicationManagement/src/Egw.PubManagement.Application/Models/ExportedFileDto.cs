using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using HotChocolate.Data;

namespace Egw.PubManagement.Application.Models;

/// <summary>
/// Exported file
/// <see cref="ExportedFile"/>
/// </summary>
public class ExportedFileDto : ITimeStampedEntity, IProjectedDto<ExportedFile>
{
    /// <summary>
    /// Unique file identifier
    /// </summary>
    [IsProjected]
    public Guid Id { get; init; }

    /// <summary>
    /// Publication ID
    /// </summary>
    [IsProjected]
    public int PublicationId { get; init; }

    /// <summary>
    /// Exported type enum
    /// </summary>
    [IsProjected]
    public ExportTypeEnum Type { get; init; }

    /// <summary> Is main export </summary>
    [IsProjected]
    public bool IsMain { get; set; }

    /// <summary> File size </summary>
    public long Size { get; init; }

    /// <summary> File URI </summary>
    public required Uri Uri { get; init; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; init; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; init; }
}