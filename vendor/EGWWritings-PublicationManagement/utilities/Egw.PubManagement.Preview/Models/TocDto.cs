using System.Text.Json.Serialization;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Preview.Models;

/// <summary>
/// Table of contents
/// </summary>
public sealed class TocDto
{
    /// <summary>
    /// Paragraph ID
    /// </summary>
    [JsonPropertyName("para_id")]
    public required ParaId ParaId { get; set; }

    /// <summary>
    /// Entry level
    /// </summary>
    [JsonPropertyName("level")]
    public required int Level { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; set; } = "";

    /// <summary>
    /// Refcode short
    /// </summary>
    [JsonPropertyName("refcode_short")]
    public required string RefCodeShort { get; set; } = "";

    /// <summary>
    /// For continuous headers points to top level header
    /// </summary>
    [JsonPropertyName("dup")]
    public required ParaId? IsDuplicateOf { get; set; }

    /// <summary>
    /// Corresponding mp3 file (if available)
    /// </summary>
    [JsonPropertyName("mp3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? Mp3Path { get; set; }

    /// <summary>
    /// Chapter/start paragraph order
    /// </summary>
    [JsonPropertyName("puborder")]
    public required int PubOrder { get; set; }
}