using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <summary>
/// Validator for <see cref="MoveFolderInput"/>
/// </summary>
public class MoveFolderValidator : AbstractValidator<MoveFolderInput>
{
    /// <inheritdoc />
    public MoveFolderValidator()
    {
        RuleFor(r => r.NewPosition)
            .NotEmpty().When(r => r.NewParent is null);
    }
}
