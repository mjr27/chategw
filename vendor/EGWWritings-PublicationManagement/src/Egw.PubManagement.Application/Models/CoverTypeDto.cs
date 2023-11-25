using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;

using HotChocolate.Data;

namespace Egw.PubManagement.Application.Models;

/// <summary> Cover type </summary>
public class CoverTypeDto : IProjectedDto<CoverType>
{
    /// <summary>
    /// Cover type code
    /// </summary>
    [IsProjected]
    public required string Code { get; init; }

    /// <summary>
    /// Cover type description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Minimum width of the cover type. If null, minimal width is not specified.
    /// </summary>
    public required int MinWidth { get; init; }

    /// <summary>
    /// Minimum width of the cover type. If null, minimal width is not specified.
    /// </summary>
    public required int MinHeight { get; init; }

    /// <summary>
    /// Number of uploaded covers of this type.
    /// </summary>
    public int CoverCount { get; set; }
}