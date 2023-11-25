using System.Text.Json.Serialization;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Preview.Models;

/// <summary>
/// Book
/// </summary>
public class BookDto
{
    /// <summary>
    /// Book Id
    /// </summary>
    [JsonPropertyName("book_id")]
    public required int BookId { get; set; }

    /// <summary>
    /// Book Code
    /// </summary>
    public required string Code { get; set; } = "";

    /// <summary>
    /// Language Code
    /// </summary>
    [JsonPropertyName("lang")]
    public required string Language { get; set; } = "";

    /// <summary>
    /// Book type
    /// </summary>
    public required BookType Type { get; set; }

    /// <summary>
    /// Book subtype (E.g Devotional)
    /// </summary>
    public required string Subtype { get; set; } = "";

    /// <summary>
    /// Book title
    /// </summary>
    public required string Title { get; init; } = "";

    /// <summary>
    /// First para_id
    /// </summary>
    [JsonPropertyName("first_para")]
    public required ParaId FirstPara { get; init; }

    /// <summary>
    /// Book author
    /// </summary>
    public required string Author { get; init; } = "";

    /// <summary>
    /// Annotation/description
    /// </summary>
    public required string Description { get; set; } = "";

    /// <summary>
    /// Page count
    /// </summary>
    [JsonPropertyName("npages")]
    public required int PageCount { get; set; }

    /// <summary>
    /// ISBN Code
    /// </summary>
    public required string? Isbn { get; set; }


    /// <summary>
    /// Publisher name
    /// </summary>
    public required string? Publisher { get; set; }

    /// <summary>
    /// Year of publication (can be empty)
    /// </summary>
    [JsonPropertyName("pub_year")]
    public required string PubYear { get; init; }

    /// <summary>
    /// Purchase link
    /// </summary>
    [JsonPropertyName("buy_link")]
    public string? BuyLink { get; } = null;

    /// <summary>
    /// Folder Id
    /// </summary>
    [JsonPropertyName("folder_id")]
    public int FolderId { get; } = -1;

    /// <summary>
    /// Folder color group
    /// </summary>
    [JsonPropertyName("folder_color_group")]
    public string FolderColorGroup { get; } = "egwwritings";

    /// <summary>
    /// Cover details
    /// </summary>
    public object? Cover => null;

    /// <summary>
    /// Downloadable file details
    /// </summary>
    public object? Files => null;

    /// <summary>
    /// Download link
    /// </summary>
    public string? Download => null;

    /// <summary>
    /// Last modified timestamp
    /// </summary>
    [JsonPropertyName("last_modified")]
    public required DateTime LastModified { get; set; }
    /// <summary>
    /// Permissions, required to read this book
    /// </summary>
    public string PermissionRequired => "authenticated";

    /// <summary>
    /// Global ordering
    /// </summary>
    public required int Sort { get; init; }

    /// <summary>
    /// Book has mp3 files
    /// </summary>
    [JsonPropertyName("is_audiobook")]
    public bool IsAudioBook => false;

    /// <summary>
    /// Bibliography reference / cite
    /// </summary>
    public string Cite { get; } = "";

    /// <summary>
    /// List of languages the book translated into
    /// </summary>
    [JsonPropertyName("translated_into")]
    public IReadOnlyCollection<string> TranslatedInto { get; } = Array.Empty<string>();

    /// <summary>
    /// Obsolete nelements field
    /// </summary>
    [Obsolete]
    [JsonPropertyName("nelements")]
    public int ObsoleteParagraphCount => 0;
}