using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters;

internal class WemlDummyInlineNode : IWemlInlineNode
{
    public WemlDummyInlineNode(IEnumerable<IWemlInlineNode> children)
    {
        Children = children.ToList();
    }

    public WemlDummyInlineNode() : this(ArraySegment<IWemlInlineNode>.Empty)
    {
    }

    public ICollection<IWemlInlineNode> Children { get; }
    public IEnumerable<IWemlNode> ChildNodes => Children;
}