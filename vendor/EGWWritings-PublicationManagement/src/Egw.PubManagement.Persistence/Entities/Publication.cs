using System.Diagnostics.CodeAnalysis;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Publication
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public sealed class Publication : ITimeStampedEntity
{
    /// <summary> Publication ID </summary>
    public int PublicationId { get; private set; }

    /// <summary> Publication type </summary>
    public PublicationType Type { get; private set; }

    /// <summary> Language code </summary>
    public string LanguageCode { get; private set; }

    /// <summary> Language navigation property </summary>
    public Language Language { get; private set; } = null!;

    /// <summary> Publication code </summary>
    public string Code { get; private set; }

    /// <summary> Publication title </summary>
    public string Title { get; private set; } = "";

    /// <summary> Publication description </summary>
    public string Description { get; set; } = "";

    /// <summary> Publication author </summary>
    public int? AuthorId { get; set; }

    /// <summary> Author navigation property </summary>
    public PublicationAuthor? Author { get; private set; }

    /// <summary> Original publication ID </summary>
    public int? OriginalPublicationId { get; set; }

    /// <summary> Original publication navigation property </summary>
    public Publication? OriginalPublication { get; set; }

    /// <summary> Year of publication </summary>
    public int? PublicationYear { get; set; }

    /// <summary> Publisher </summary>
    public string Publisher { get; set; } = "";

    /// <summary> ISBN Code </summary>
    public string? Isbn { get; set; }

    /// <summary> Purchase URI </summary>
    public Uri? PurchaseLink { get; set; }

    /// <summary> Page count </summary>
    public int? PageCount { get; set; }


    /// <summary> Publication placement </summary>
    public PublicationPlacement? Placement { get; set; }

    /// <summary> Paragraphs navigation property </summary>
    public List<Paragraph> Paragraphs { get; private set; } = new();

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary> Covers navigation property </summary>
    public List<Cover> Covers { get; private set; } = new();

    /// <summary>
    /// Creates a new publication
    /// </summary>
    public Publication(int publicationId, PublicationType type, string languageCode, string code,
        DateTimeOffset createdAt)
    {
        PublicationId = publicationId;
        Type = type;
        LanguageCode = languageCode;
        Code = code;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Changes publication title
    /// </summary>
    /// <param name="title">Publication title</param>
    /// <param name="updatedAt">Date of update</param>
    public Publication ChangeTitle(string title, DateTimeOffset updatedAt)
    {
        Title = title;
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    ///  Changes publication details
    /// </summary>
    /// <param name="code">New publication code</param>
    /// <param name="title">New publication title</param>
    /// <param name="description">New description</param>
    /// <param name="publisher">Publisher</param>
    /// <param name="pageCount">Page count</param>
    /// <param name="publicationYear">Publication year</param>
    /// <param name="updatedAt"></param>
    /// <returns></returns>
    public Publication ChangeDetails(
        string code,
        string title,
        string description,
        string publisher,
        int? pageCount,
        int? publicationYear, DateTimeOffset updatedAt)
    {
        Code = code;
        Title = title;
        Description = description;
        Publisher = publisher;
        PageCount = pageCount;
        PublicationYear = publicationYear;
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Sets publication ISBN
    /// </summary>
    public Publication SetPurchaseLink(Uri? purchaseLink, DateTimeOffset updatedAt)
    {
        PurchaseLink = purchaseLink;
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    ///  Changes publication metadata
    /// </summary>
    /// <param name="code">New publication code</param>
    /// <param name="title">New publication title</param>
    /// <param name="description">New description</param>
    /// <param name="authorId">New authorId</param>
    /// <param name="publisher">Publisher</param>
    /// <param name="pageCount">Page count</param>
    /// <param name="publicationYear">Publication year</param>
    /// <param name="isbn">Publication Isbn</param>
    /// <param name="updatedAt"></param>
    /// <returns></returns>
    public Publication ChangeMetadata(
        string code,
        string title,
        string description,
        int? authorId,
        string publisher,
        int? pageCount,
        int? publicationYear,
        string isbn,
        DateTimeOffset updatedAt)
    {
        Code = code;
        Title = title;
        Description = description;
        AuthorId = authorId;
        Publisher = publisher;
        PageCount = pageCount;
        PublicationYear = publicationYear;
        Isbn = isbn;
        UpdatedAt = updatedAt;
        return this;
    }
}