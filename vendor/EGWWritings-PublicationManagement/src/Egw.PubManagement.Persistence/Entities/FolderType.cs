using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Folder Type
/// </summary>
public class FolderType : ITimeStampedEntity
{
    /// <summary>
    /// Folder type Id
    /// </summary>
    public string FolderTypeId { get; private set; }

    /// <summary>
    /// Folder type name
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Allowed publication types
    /// </summary>
    public PublicationType[] AllowedTypes { get; private set; }

    /// <summary>
    /// Created at
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Updated at
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new folder type
    /// </summary>
    public FolderType(string folderTypeId, string title, DateTimeOffset createdAt,
        params PublicationType[] allowedTypes)
    {
        FolderTypeId = folderTypeId;
        Title = title;
        AllowedTypes = allowedTypes;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    // ReSharper disable once UnusedMember.Local
    private FolderType()
    {
        FolderTypeId = "";
        Title = "";
        AllowedTypes = Array.Empty<PublicationType>();
    }
}