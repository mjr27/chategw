namespace Egw.PubManagement.LatexExport.Nodes;

/// <summary>
/// Grouped node
/// </summary>
public class LatexGroupNode : ILatexNode
{
    /// <summary>
    /// Group node children
    /// </summary>
    public IReadOnlyCollection<ILatexNode> Children { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public LatexGroupNode(IEnumerable<ILatexNode> children)
    {
        Children = children.ToArray();
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public LatexGroupNode(params ILatexNode[] children)
    {
        Children = children;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Join("\n", Children);
    }
}