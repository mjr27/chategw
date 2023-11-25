using AngleSharp;
using AngleSharp.Dom;
namespace Egw.PubManagement.LegacyImport.Internal;

internal sealed class NodeWrapper
{
    private readonly INode _node;

    public NodeWrapper(INode node)
    {
        _node = node;
    }

    public override string ToString() => _node.ToHtml();
}