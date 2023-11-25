using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using HotChocolate.Data;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Models;

/// <summary>
/// DTO for <see cref="Publication"/>
/// </summary>
public sealed class PublicationDto : IProjectedDto<Publication>
{
    /// <summary>
    /// Publication ID
    /// </summary>
    [IsProjected]
    public int PublicationId { get; init; }

    /// <summary>
    /// Publication type
    /// </summary>
    [IsProjected]
    public PublicationType Type { get; init; }

    /// <summary>
    /// Language code
    /// </summary>
    [IsProjected]
    public string LanguageCode { get; init; } = "";

    /// <summary>
    /// Publication code
    /// </summary>
    [IsProjected]
    public string Code { get; init; } = "";

    /// <summary>
    /// Publication title
    /// </summary>
    public string Title { get; init; } = "";

    /// <summary>
    /// Publication description
    /// </summary>
    public string Description { get; init; } = "";

    /// <summary>
    /// Folder ID
    /// </summary>
    [IsProjected]
    public int? FolderId { get; init; }

    /// <summary>
    /// Order in folder
    /// </summary>
    [IsProjected]
    public int Order { get; init; }

    /// <summary>
    /// Publication author
    /// </summary>
    [IsProjected]
    public int? AuthorId { get; init; }

    /// <summary>
    /// Original publication ID
    /// </summary>
    [IsProjected]
    public int? OriginalPublicationId { get; init; }

    /// <summary>
    /// Year of publication
    /// </summary>
    public int? PublicationYear { get; init; }

    /// <summary>
    /// Publisher
    /// </summary>
    public string Publisher { get; init; } = "";

    /// <summary>
    /// ISBN Code
    /// </summary>
    public string? Isbn { get; init; }

    /// <summary>
    /// Purchase URI
    /// </summary>
    public Uri? PurchaseLink { get; init; }

    /// <summary>
    /// Page count
    /// </summary>
    public int? PageCount { get; init; }

    /// <summary>
    /// Created at
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Updated at
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
    /// <summary>
    /// Permission
    /// </summary>
    public PublicationPermissionEnum Permission { get; set; }
}