namespace Egw.PubManagement.LatexExport.Nodes;

/// <summary>
/// Text node for latex output
/// </summary>
public class LatexTextNode : ILatexNode
{
    /// <summary>
    /// Node text
    /// </summary>
    public string Text { get; }

    /// <inheritdoc />
    public override string ToString() => Text;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="text">Node text. Already escaped</param>
    public LatexTextNode(string text)
    {
        Text = text;
    }
}