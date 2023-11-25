using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// Link parser
/// </summary>
public interface ILinkParser
{
    /// <summary>
    /// Resolves link reference
    /// </summary>
    /// <param name="element">Element</param>
    /// <param name="context">Parsing context</param>
    /// <param name="paraId">Paragraph ID</param>
    /// <param name="isBible">Is bible link</param>
    /// <param name="title">Output link title</param>
    /// <returns></returns>
    bool TryResolveReference(IElement element,
        ILegacyParserContext context,
        out ParaId paraId,
        out bool isBible,
        out string? title);
}