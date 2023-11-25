using Egw.PubManagement.Application.Models.Internal;

using HotChocolate.Language;
using HotChocolate.Types;

namespace Egw.PubManagement.Application.GraphQl.Scalars;

/// <inheritdoc />
public class Mp3FileNameType : ScalarType<Mp3FileName, StringValueNode>
{
    /// <inheritdoc />
    public Mp3FileNameType() : this(nameof(Mp3FileName), BindingBehavior.Implicit)
    {
    }

    /// <inheritdoc />
    public Mp3FileNameType(string name, BindingBehavior bind = BindingBehavior.Explicit) : base(name, bind)
    {
        Description = "Paragraph ID";
    }

    /// <inheritdoc />
    protected override bool IsInstanceOfType(Mp3FileName runtimeValue) => true;

    /// <inheritdoc />
    protected override bool IsInstanceOfType(StringValueNode valueSyntax) =>
        Mp3FileName.TryParse(valueSyntax.Value, out _);

    /// <inheritdoc />
    public override IValueNode ParseResult(object? resultValue) => this.ParseValue(resultValue);

    /// <inheritdoc />
    protected override Mp3FileName ParseLiteral(StringValueNode valueSyntax) => Mp3FileName.Parse(valueSyntax.Value);

    /// <inheritdoc />
    protected override StringValueNode ParseValue(Mp3FileName runtimeValue) => new(runtimeValue.ToString());
}