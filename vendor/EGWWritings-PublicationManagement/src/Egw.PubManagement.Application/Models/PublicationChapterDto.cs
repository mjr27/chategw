using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;

using HotChocolate.Data;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Models;

/// <summary>
/// Publication chapter
/// <see cref="PublicationChapter"/>
/// </summary>
public class PublicationChapterDto : ITimeStampedEntity, IProjectedDto<PublicationChapter>
{
    /// <summary>
    /// Publication id
    /// </summary>
    [IsProjected]
    public int PublicationId { get; init; }

    /// <summary>
    /// Chapter level
    /// </summary>
    public int Level { get; init; }

    /// <summary> Starting paragraph ID for a chapter </summary>
    [IsProjected]
    public ParaId ParaId { get; init; }
    /// <summary>
    /// Chapter identifier
    /// </summary>
    [IsProjected]
    public ParaId ChapterId { get; init; }

    /// <summary>
    /// Chapter order
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Chapter title
    /// </summary>
    public string Title { get; init; } = "";

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; init; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; init; }
}