using System.Text.Json.Serialization;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Preview.Models;

/// <summary>
/// Paragraph
/// </summary>
public class LegacyParagraphDto
{
    /// <summary>
    /// Paragraph ID
    /// </summary>
    [JsonPropertyName("para_id")]
    public ParaId ParaId { get; set; }

    /// <summary>
    /// Previous paragraph id
    /// </summary>
    [JsonPropertyName("id_prev")]
    public ParaId? PreviousParaId { get; set; }

    /// <summary>
    /// Next paragraph id
    /// </summary>
    [JsonPropertyName("id_next")]
    public ParaId? NextParaId { get; set; }

    /// <summary>
    /// Reference code 1
    /// </summary>
    [JsonPropertyName("refcode_1")]
    public required string RefCode1 { get; set; } = null!;

    /// <summary>
    /// Reference code 2
    /// </summary>
    [JsonPropertyName("refcode_2")]
    public required string RefCode2 { get; set; }

    /// <summary>
    /// Reference code 3
    /// </summary>
    [JsonPropertyName("refcode_3")]
    public required string RefCode3 { get; set; }

    /// <summary>
    /// Reference code 4
    /// </summary>
    [JsonPropertyName("refcode_4")]
    public required string RefCode4 { get; set; }

    /// <summary>
    /// Short reference code
    /// </summary>

    [JsonPropertyName("refcode_short")]
    public required string RefCodeShort { get; set; }

    /// <summary>
    /// Long/full reference code
    /// </summary>
    [JsonPropertyName("refcode_long")]
    public required string RefCodeLong { get; set; }

    /// <summary>
    /// Element type (html tag name)
    /// </summary>
    [JsonPropertyName("element_type")]
    public required string ElementType { get; set; }

    /// <summary>
    /// Element subtype (html class name)
    /// </summary>
    [JsonPropertyName("element_subtype")]
    public required string ElementSubType { get; set; }

    /// <summary>
    /// Paragraph contents
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; } = "";

    /// <summary>
    /// Paragraph order
    /// </summary>
    [JsonPropertyName("puborder")]
    public required int PubOrder { get; init; }

    /// <summary>
    /// List of translations to other languages
    /// </summary>
    [JsonPropertyName("translations")]
    public IReadOnlyCollection<object> Translations { get; } = Array.Empty<object>();
}