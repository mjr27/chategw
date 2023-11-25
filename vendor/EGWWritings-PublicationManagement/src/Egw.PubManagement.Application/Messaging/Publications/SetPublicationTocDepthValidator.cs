using FluentValidation;

namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Validator for <see cref="SetPublicationTocDepthInput"/>
/// </summary>
public class SetPublicationTocDepthValidator : AbstractValidator<SetPublicationTocDepthInput>
{
    /// <inheritdoc />
    public SetPublicationTocDepthValidator()
    {
        RuleFor(r => r.TocDepth)
            .GreaterThan(0).When(x => x.TocDepth is not null);
    }
}