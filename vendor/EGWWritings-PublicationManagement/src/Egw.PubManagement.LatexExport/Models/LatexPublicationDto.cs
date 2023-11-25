
using WhiteEstate.DocFormat.Enums;

namespace Egw.PubManagement.LatexExport.Models;

/// <summary>
/// Latex publication data transfer object
/// </summary>
public class LatexPublicationDto
{
    /// <summary>
    /// Publication ID
    /// </summary>
    public required int PublicationId { get; set; }

    /// <summary>
    /// Language code
    /// </summary>
    public required string LanguageCode { get; set; }

    /// <summary>
    /// Publication type
    /// </summary>
    public required PublicationType Type { get; set; }

    /// <summary>
    /// Publication title
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Publication author name
    /// </summary>
    public required string? Author { get; set; }

    /// <summary>
    /// Publication year
    /// </summary>
    public required int? PublicationYear { get; set; }
}