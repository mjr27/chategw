using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Internal;

internal class LegacyContext : ILegacyParserContext
{
    public WemlDocumentHeader Header { get; }
    public IHtmlDocument Document { get; }

    public LegacyContext(WemlDocumentHeader header, IHtmlDocument document)
    {
        Header = header;
        Document = document;
    }

    public void Report(WarningLevel level, INode node, string errorMessage)
    {
        if (_root is null)
        {
            throw new ArgumentException("No context specified");
        }

        _warnings.Add(new CollectedWarning(level, node, _root, errorMessage));
    }

    public void SetContext(IElement currentElement)
    {
        _root = currentElement;
    }

    public IEnumerable<CollectedWarning> Serialize(WarningLevel minLevel = WarningLevel.Information) => _warnings
        .Where(r => r.Level >= minLevel);


    private IElement? _root;

    private readonly List<CollectedWarning> _warnings = new();
}