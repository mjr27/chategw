namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// WE Language
/// </summary>
public sealed class Language : ITimeStampedEntity
{
    /// <summary>
    /// Unique publication code (eng)
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// EGW 2 (3) letter code (en)
    /// </summary>
    public string EgwCode { get; private set; }

    /// <summary>
    /// BCP47 code (en-US)
    /// </summary>
    public string Bcp47Code { get; private set; }

    /// <summary>
    /// Is right to left
    /// </summary>
    public bool IsRightToLeft { get; private set; }

    /// <summary>
    /// Root folder Id
    /// </summary>
    public int? RootFolderId { get; set; }

    /// <summary>
    /// Root folder navigation property
    /// </summary>
    public Folder? RootFolder { get; set; }

    /// <summary>
    /// Language title
    /// </summary>
    public string Title { get; private set; } = "";

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new language
    /// </summary>
    public Language(string code, string egwCode, string bcp47Code, bool isRightToLeft,
        string title,
        DateTimeOffset createdAt)
    {
        Code = code.ToLowerInvariant();
        EgwCode = egwCode.ToLowerInvariant();
        Bcp47Code = bcp47Code.ToLowerInvariant();
        IsRightToLeft = isRightToLeft;
        Title = title;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Sets EGW Code
    /// </summary>
    /// <param name="egwCode"></param>
    /// <param name="updatedAt"></param>
    /// <returns></returns>
    public Language SetEgwCode(string egwCode, DateTimeOffset updatedAt)
    {
        EgwCode = egwCode.ToLowerInvariant();
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Sets EGW Code
    /// </summary>
    /// <param name="code"></param>
    /// <param name="updatedAt"></param>
    /// <returns></returns>
    public Language SetBcp47Code(string code, DateTimeOffset updatedAt)
    {
        Bcp47Code = code.ToLowerInvariant();
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Sets EGW Code
    /// </summary>
    /// <param name="title"></param>
    /// <param name="updatedAt"></param>
    /// <returns></returns>
    public Language SetTitle(string title, DateTimeOffset updatedAt)
    {
        Title = title;
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Sets root folder id
    /// </summary>
    /// <param name="rootFolderId"></param>
    /// <param name="updatedAt"></param>
    /// <returns></returns>
    public Language SetRootFolderId(int? rootFolderId, DateTimeOffset updatedAt)
    {
        RootFolderId = rootFolderId;
        UpdatedAt = updatedAt;
        return this;
    }

    // ReSharper disable once UnusedMember.Local
    private Language()
    {
        Code = "";
        EgwCode = "";
        Bcp47Code = "";
    }
}