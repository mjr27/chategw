using FluentValidation;

namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Validator for <see cref="MovePublicationInput"/>
/// </summary>
public class MovePublicationValidator : AbstractValidator<MovePublicationInput>
{
    /// <inheritdoc />
    public MovePublicationValidator()
    {
        RuleFor(r => r.FolderId)
            .GreaterThan(0);

        RuleFor(r => r.Order)
            .GreaterThan(0).When(x => x.Order is not null);
    }
}