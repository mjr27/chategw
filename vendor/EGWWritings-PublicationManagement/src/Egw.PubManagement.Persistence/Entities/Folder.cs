namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Folder
/// </summary>
public sealed class Folder : ITimeStampedEntity
{
    /// <summary>
    /// Materialized path element length
    /// </summary>
    public const int MaterializedPathElementLength = 5;
    /// <summary>
    /// Materialized path separator
    /// </summary>
    public const char MaterializedPathSeparator = '/';

    /// <summary>
    /// Unique folder id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Folder parent ID
    /// </summary>
    public int? ParentId { get; private set; }

    /// <summary>
    /// Order
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Global order
    /// </summary>
    public int GlobalOrder { get; private set; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Folder type ID
    /// </summary>
    public string TypeId { get; private set; }

    /// <summary>
    /// Folder Type
    /// </summary>
    public FolderType Type { get; private set; } = null!;

    /// <summary>
    /// Folder parent navigation property
    /// </summary>
    public Folder? Parent { get; private set; }

    /// <summary>
    /// Folder children navigation property
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Folder> Children { get; private set; } = new();

    /// <summary>
    /// Publication placements
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<PublicationPlacement> Placements { get; private set; } = new();

    /// <summary>
    /// Materialized path
    /// </summary>
    public string MaterializedPath { get; set; } = "";

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Date of last modification
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    // ReSharper disable once UnusedMember.Local
    private Folder()
    {
        Title = "";
        TypeId = null!;
    }

    /// <summary>
    /// Creates a new folder
    /// </summary>
    public Folder(string title, int? parentId, int order, string typeId, DateTimeOffset createdAt)
    {
        Order = order;
        Title = title;
        ParentId = parentId;
        TypeId = typeId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Sets folder position
    /// </summary>
    /// <param name="parentId">Parent Id</param>
    /// <param name="order">New order</param>
    /// <param name="moment">Current moment</param>
    public void SetPosition(int? parentId, int order, DateTimeOffset moment)
    {
        ParentId = parentId;
        Order = order;
        UpdatedAt = moment;
    }

    /// <summary>
    /// Sets folder position
    /// </summary>
    public void SetGlobalPosition(int globalOrder)
    {
        GlobalOrder = globalOrder;
    }

    /// <summary>
    /// Sets folder title
    /// </summary>
    /// <param name="title"></param>
    /// <param name="moment"></param>
    public void SetTitle(string title, DateTimeOffset moment)
    {
        Title = title;
        UpdatedAt = moment;
    }

    /// <summary>
    /// Sets folder type
    /// </summary>
    /// <param name="newType">Type id of a new type</param>
    /// <param name="moment"></param>
    public void SetType(string newType, DateTimeOffset moment)
    {
        TypeId = newType;
        UpdatedAt = moment;
    }
}