using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;

namespace Egw.PubManagement.Application.Models;

/// <inheritdoc />
public class ArchivedPublicationDto : IProjectedDto<ArchivedPublication>
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Publication identifier
    /// </summary>
    public int PublicationId { get; private set; }

    /// <summary>
    /// Archived publication version
    /// </summary>
    public DateTimeOffset ArchivedAt { get; private set; }
}