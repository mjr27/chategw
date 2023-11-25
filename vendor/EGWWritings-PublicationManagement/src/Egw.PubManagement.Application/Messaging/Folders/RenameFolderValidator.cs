using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <summary>
/// Validator for <see cref="RenameFolderInput"/>
/// </summary>
public class RenameFolderValidator : AbstractValidator<RenameFolderInput>
{
    /// <inheritdoc />
    public RenameFolderValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(100);
    }
}