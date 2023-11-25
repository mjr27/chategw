using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Messaging.Paragraphs;

/// <summary>
/// Update paragraph content
/// </summary>
public class SetParagraphContentInput : IApplicationCommand
{
    /// <summary>
    /// Paragraph to update
    /// </summary>
    public required ParaId ParaId { get; init; }
    /// <summary>
    /// Content to update
    /// </summary>
    public required String Content { get; init; }
}
