namespace Egw.PubManagement.LatexExport.Nodes;

/// <summary>
/// Latex macro node
/// </summary>
public sealed class LatexMacroNode : ILatexNode
{
    /// <summary>
    /// Macro name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Macro options
    /// </summary>
    public Dictionary<string, string?> Options { get; init; } = new();

    /// <summary>
    /// Macro children
    /// </summary>
    public List<ILatexNode> Children { get; init; } = new();

    /// <summary>
    /// Create empty macro node
    /// </summary>
    /// <param name="name"></param>
    public LatexMacroNode(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Create a macro node with a single text node as a child
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public LatexMacroNode(string name, string value)
    {
        Name = name;
        Children.Add(new LatexTextNode(value));
    }

    /// <inheritdoc />
    public override string ToString()
    {
        string result = "\\" + Name;
        if (Options.Count > 0)
        {
            result += '[' + BuildOptionalArguments(Options) + ']';
        }

        if (Children.Any())
        {
            result += '{' + string.Join("", Children.Select(r => r)) + '}';
        }
        else
        {
            result += "{}";
        }

        return result;
    }

    private static string BuildOptionalArguments(Dictionary<string, string?> optionalArguments)
    {
        var result = new List<string>();
        foreach ((string k, string? v) in optionalArguments)
        {
            result.Add(v is null ? k : $"{k}={{{v}}}");
        }

        return string.Join(',', result);
    }
}