using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Paragraph link
/// </summary>
public class ParagraphLink
{
    /// <summary>
    /// Paragraph ID
    /// </summary>
    public ParaId ParaId { get; private set; }


    /// <summary>
    /// Publication ID
    /// </summary>
    public int PublicationId { get; private set; }

    /// <summary>
    /// Element Id
    /// </summary>
    public int ElementId { get; private set; }

    /// <summary>
    /// Original paragraph ID
    /// </summary>
    public ParaId OriginalParaId { get; private set; }


    /// <summary>
    /// Original publication ID
    /// </summary>
    public int OriginalPublicationId { get; private set; }

    /// <summary>
    /// Original element ID
    /// </summary>
    public int OriginalElementId { get; private set; }

    /// <summary>
    /// Paragrah link
    /// </summary>
    /// <param name="paraId">Source paragraph ID</param>
    /// <param name="originalParaId">Original paragraph ID</param>
    public ParagraphLink(ParaId paraId, ParaId originalParaId)
    {
        ParaId = paraId;
        OriginalParaId = originalParaId;
        PublicationId = paraId.PublicationId;
        ElementId = paraId.ElementId;
        OriginalPublicationId = originalParaId.PublicationId;
        OriginalElementId = originalParaId.ElementId;
    }
}