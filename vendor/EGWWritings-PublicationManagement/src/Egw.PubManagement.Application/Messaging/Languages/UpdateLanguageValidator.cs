using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Languages;

/// <summary>
/// Validator for <see cref="UpdateLanguageInput"/>
/// </summary>
public class UpdateLanguageValidator : AbstractValidator<UpdateLanguageInput>
{
    /// <inheritdoc />
    public UpdateLanguageValidator()
    {
        RuleFor(r => r.Code)
            .NotEmpty()
            .MaximumLength(5);
        RuleFor(r => r.EgwCode)
            .MaximumLength(3).When(r => r.EgwCode is not null)
            .MinimumLength(1).When(r => r.EgwCode is not null);
        RuleFor(r => r.Bcp47Code)
            .MinimumLength(1).When(r => r.Bcp47Code is not null)
            .MaximumLength(10).When(r => r.Bcp47Code is not null)
            .Matches(CreateLanguageValidator.Bcp47Regex()).When(r => r.Bcp47Code is not null);
        RuleFor(r => r.Title)
            .MinimumLength(1).When(r => r.Title is not null);
    }


}