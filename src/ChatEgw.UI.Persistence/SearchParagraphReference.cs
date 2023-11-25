using System.ComponentModel.DataAnnotations;

namespace ChatEgw.UI.Persistence;

public class SearchParagraphReference
{
    [Key] public long Id { get; set; }
    public long ParagraphId { get; set; }

    /// <summary> Paragraph navigation property </summary>
    public SearchParagraph Paragraph { get; set; } = null!;

    public required string ReferenceCode { get; set; }
}