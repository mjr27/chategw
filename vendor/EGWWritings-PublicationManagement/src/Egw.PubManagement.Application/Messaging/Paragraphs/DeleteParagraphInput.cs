using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Messaging.Paragraphs;

/// <summary>
/// Deletes a paragraph
/// </summary>
public class DeleteParagraphInput : IApplicationCommand
{
    /// <summary>
    /// Paragraph to deleting
    /// </summary>
    public required ParaId ParaId { get; init; }
}