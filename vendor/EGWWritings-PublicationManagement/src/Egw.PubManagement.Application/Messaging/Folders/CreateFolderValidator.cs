using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <summary>
/// Validator for <see cref="CreateFolderInput"/>
/// </summary>
public class CreateFolderValidator : AbstractValidator<CreateFolderInput>
{
    /// <inheritdoc />
    public CreateFolderValidator()
    {
        RuleFor(r => r.ParentId)
            .GreaterThan(0);
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(100);
    }
}