namespace Egw.PubManagement.LatexExport.Models;

/// <summary>
/// Paragraph data transfer object
/// </summary>
public class LatexParagraphDto
{
    /// <summary>
    /// Paragraph id, unique within publication
    /// </summary>
    public required int ParagraphId { get; set; }

    /// <summary>
    /// Heading level. null for skipped, 0 for paragraph, 1-6 for title 
    /// </summary>
    public required int? HeadingLevel { get; set; }

    /// <summary>
    /// WeML content
    /// </summary>
    public required string WemlContent { get; set; }
}