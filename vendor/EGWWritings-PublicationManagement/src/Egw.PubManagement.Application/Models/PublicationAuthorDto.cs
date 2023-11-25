using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;

using HotChocolate.Data;
namespace Egw.PubManagement.Application.Models;

/// <summary>
/// Publication author
/// <see cref="PublicationAuthor"/>
/// </summary>
public class PublicationAuthorDto : ITimeStampedEntity, IProjectedDto<PublicationAuthor>
{
    /// <summary>
    /// Unique author ID
    /// </summary>
    [IsProjected]
    public int Id { get; init; }

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; init; } = "";

    /// <summary>
    /// Middle name
    /// </summary>
    public string MiddleName { get; init; } = "";

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; init; } = "";

    /// <summary>
    /// Author code
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Author biography
    /// </summary>
    public string? Biography { get; init; }

    /// <summary>
    /// Birth year
    /// </summary>
    public int? BirthYear { get; init; }

    /// <summary>
    /// Death year
    /// </summary>
    public int? DeathYear { get; init; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; init; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; init; }
}