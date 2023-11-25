using System.Text.RegularExpressions;

using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Languages;

/// <summary>
/// Validator for <see cref="CreateLanguageInput"/>
/// </summary>
public partial class CreateLanguageValidator : AbstractValidator<CreateLanguageInput>
{
    /// <inheritdoc />
    public CreateLanguageValidator()
    {
        RuleFor(r => r.Code)
            .NotEmpty()
            .MaximumLength(5);
        RuleFor(r => r.EgwCode)
            .NotEmpty()
            .MaximumLength(3);
        RuleFor(r => r.Bcp47Code)
            .NotEmpty()
            .MaximumLength(10)
            .Matches(Bcp47Regex());
        RuleFor(r => r.Title)
            .NotEmpty();
        RuleFor(r => r.Direction)
            .IsInEnum();
    }

    [GeneratedRegex("^[a-zA-Z]{2,3}(-[a-zA-Z0-9]{2,8})?$")]
    internal static partial Regex Bcp47Regex();
}