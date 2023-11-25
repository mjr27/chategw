using FluentValidation;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Messaging.Paragraphs
{
    /// <summary>
    /// Validator for <see cref="AddParagraphInput"/>
    /// </summary>
    public class AddParagraphValidator : AbstractValidator<AddParagraphInput>
    {
        /// <inheritdoc />
        public AddParagraphValidator()
        {
            RuleFor(r => r.Content)
                .NotNull()
                .NotEmpty()
                .MustAsync(WemlExtensions.ValidateAsync);
        }
    }
}
