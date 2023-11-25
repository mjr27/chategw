using System.Text.Json.Serialization;

using Egw.PubManagement.Persistence.Enums;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Models.Internal;

/// <summary>
/// Mp3 file information
/// </summary>
public class Mp3ManifestType
{
    /// <summary>
    /// File name
    /// </summary>
    [JsonPropertyName("file")]
    [JsonConverter(typeof(Mp3FileNameConverter))]
    public required Mp3FileName File { get; init; }

    /// <summary>
    /// Publication Id
    /// </summary>
    [JsonPropertyName("book_id")]
    public int BookId { get; init; }

    /// <summary>
    /// Element Id
    /// </summary>
    [JsonPropertyName("element_id")]
    public int ElementId { get; init; }

    /// <summary>
    /// Para Id
    /// </summary>
    [JsonPropertyName("para_id")]
    [JsonConverter(typeof(ParaIdNullableConverter))]
    public required ParaId? ParaId { get; set; }

    /// <summary>
    /// Voice type
    /// </summary>
    [JsonPropertyName("voice")]
    public VoiceTypeEnum Voice { get; init; }

    /// <summary>
    /// File size
    /// </summary>
    [JsonPropertyName("size")]
    public required long Size { get; init; }

    /// <summary>
    /// Mp3 duration
    /// </summary>
    [JsonPropertyName("duration")]
    public required long Duration { get; init; }
}