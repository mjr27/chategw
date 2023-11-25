using HotChocolate.Language;
using HotChocolate.Types;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.GraphQl.Scalars;

/// <inheritdoc />
public class ParaIdType : ScalarType<ParaId, StringValueNode>
{
    /// <inheritdoc />
    public ParaIdType() : this(nameof(ParaId), BindingBehavior.Implicit)
    {
    }

    /// <inheritdoc />
    public ParaIdType(string name, BindingBehavior bind = BindingBehavior.Explicit) : base(name, bind)
    {
        Description = "Paragraph ID";
    }

    /// <inheritdoc />
    protected override bool IsInstanceOfType(ParaId runtimeValue) => true;

    /// <inheritdoc />
    protected override bool IsInstanceOfType(StringValueNode valueSyntax) => ParaId.TryParse(valueSyntax.Value, out _);

    /// <inheritdoc />
    public override IValueNode ParseResult(object? resultValue) => this.ParseValue(resultValue);

    /// <inheritdoc />
    protected override ParaId ParseLiteral(StringValueNode valueSyntax) => ParaId.Parse(valueSyntax.Value);

    /// <inheritdoc />
    protected override StringValueNode ParseValue(ParaId runtimeValue) => new(runtimeValue.ToString());
}