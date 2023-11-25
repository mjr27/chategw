namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary> Updates publication metadata </summary>
public class SetPublicationMetadataInput : IApplicationCommand
{
    /// <summary>Publication id</summary>
    public required int PublicationId { get; init; }
    /// <summary> Publication code </summary>
    public required string Code { get; init; }
    /// <summary> Publication title </summary>
    public required string Title { get; init; }
    /// <summary> Publication description </summary>
    public required string Description { get; init; }
    /// <summary> Author Id</summary>
    public required int? AuthorId { get; init; }
    /// <summary> Publisher </summary>
    public required string Publisher { get; init; }
    /// <summary> Page count </summary>
    public required int? PageCount { get; init; }
    /// <summary> Publication year </summary>
    public required int? PublicationYear { get; init; }
    /// <summary> ISBN code </summary>
    public required string Isbn { get; init; }
    /// <summary> Purchase link </summary>
    public Uri? PurchaseLink { get; init; }
}