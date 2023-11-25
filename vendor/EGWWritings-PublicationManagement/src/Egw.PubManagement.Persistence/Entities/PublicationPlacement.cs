using Egw.PubManagement.Persistence.Enums;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Folder publication placement
/// </summary>
public class PublicationPlacement : ITimeStampedEntity
{
    /// <summary>
    /// Publication ID
    /// </summary>
    public int PublicationId { get; private set; }

    /// <summary>
    /// Publication navigation property
    /// </summary>
    public Publication Publication { get; private set; } = null!;

    /// <summary>
    /// Folder ID
    /// </summary>
    public int FolderId { get; private set; }

    /// <summary>
    /// Folder navigation property
    /// </summary>
    public Folder Folder { get; private set; } = null!;

    /// <summary>
    /// Publication order in folder
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    ///  Publication permission
    /// </summary>
    public PublicationPermissionEnum Permission { get; private set; } = PublicationPermissionEnum.Hidden;

    /// <summary>
    /// Max depth for TOC
    /// </summary>
    public int? TocDepth { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }


    /// <summary>
    /// Folder publication placement
    /// </summary>
    public PublicationPlacement(int publicationId, int folderId, int order,
        DateTimeOffset createdAt)
    {
        PublicationId = publicationId;
        FolderId = folderId;
        Order = order;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Modifies permission
    /// </summary>
    /// <param name="permission">New required permission</param>
    /// <param name="moment">Current moment</param>
    /// <returns></returns>
    public PublicationPlacement ChangePermission(PublicationPermissionEnum permission, DateTimeOffset moment)
    {
        Permission = permission;
        UpdatedAt = moment;
        return this;
    }

    /// <summary>
    /// Modifies placement
    /// </summary>
    /// <param name="folderId">New folder id</param>
    /// <param name="order">New order</param>
    /// <param name="moment">Current moment</param>
    /// <returns></returns>
    public PublicationPlacement ChangePlacement(int folderId, int order, DateTimeOffset moment)
    {
        FolderId = folderId;
        Order = order;
        UpdatedAt = moment;
        return this;
    }

    /// <summary>
    /// Modifies toc depth
    /// </summary>
    /// <param name="tocDepth">New toc depth</param>
    /// <param name="moment">Current moment</param>
    /// <returns></returns>
    public PublicationPlacement ChangeTocDepth(int? tocDepth, DateTimeOffset moment)
    {
        TocDepth = tocDepth;
        UpdatedAt = moment;
        return this;
    }

}