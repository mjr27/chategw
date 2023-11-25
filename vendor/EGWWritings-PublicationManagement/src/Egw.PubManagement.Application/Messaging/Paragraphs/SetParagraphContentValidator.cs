using FluentValidation;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Messaging.Paragraphs
{
    /// <summary>
    /// Validator for <see cref="SetParagraphContentInput"/>
    /// </summary>
    public class SetParagraphContentValidator : AbstractValidator<SetParagraphContentInput>
    {
        /// <inheritdoc />
        public SetParagraphContentValidator()
        {
            RuleFor(r => r.ParaId)
                .NotNull()
                .Must(paraId => paraId.IsEmpty == false);

            RuleFor(r => r.Content)
                .NotNull()
                .NotEmpty()
                .MustAsync(WemlExtensions.ValidateAsync);
        }
    }
}
