using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using HotChocolate.Data;
namespace Egw.PubManagement.Application.Models;

/// <summary>
/// Publications series DTO
/// <see cref="PublicationSeries"/>
/// </summary>
public class PublicationSeriesDto : IProjectedDto<PublicationSeries>
{
    /// <summary>
    /// Series code
    /// </summary>
    [IsProjected]
    public string Code { get; init; } = "";

    /// <summary>
    /// Series title
    /// </summary>
    public string Title { get; init; } = "";

    /// <summary>
    /// Series type
    /// </summary>
    [IsProjected]
    public SeriesTypeEnum Type { get; init; }

    /// <summary>
    /// List of publications in series
    /// </summary>
    [IsProjected]
    public int[] Publications { get; init; } = Array.Empty<int>();

    /// <summary>
    /// Created at
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Updated at
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}