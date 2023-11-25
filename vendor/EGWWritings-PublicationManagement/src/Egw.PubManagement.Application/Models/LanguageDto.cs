using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;

using HotChocolate.Data;
namespace Egw.PubManagement.Application.Models;

/// <summary>
/// DTO for <see cref="Language"/>
/// </summary>
public sealed class LanguageDto : IProjectedDto<Language>
{
    /// <summary>
    /// Unique publication code (eng)
    /// </summary>
    [IsProjected]
    public string Code { get; init; } = "";

    /// <summary>
    /// EGW 2 (3) letter code (en)
    /// </summary>
    public string EgwCode { get; init; } = "";

    /// <summary>
    /// BCP47 code (en-US)
    /// </summary>
    public string Bcp47Code { get; init; } = "";

    /// <summary>
    /// Is right to left
    /// </summary>
    public bool IsRightToLeft { get; init; }

    /// <summary>
    /// Root folder Id
    /// </summary>
    [IsProjected]
    public int? RootFolderId { get; init; }

    /// <summary>
    /// Language title
    /// </summary>
    public string Title { get; init; } = "";

    /// <summary>
    /// Created at
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Updated at
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}