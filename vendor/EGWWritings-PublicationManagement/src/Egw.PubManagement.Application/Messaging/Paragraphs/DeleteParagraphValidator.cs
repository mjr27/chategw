using FluentValidation;

namespace Egw.PubManagement.Application.Messaging.Paragraphs
{
    /// <summary>
    /// Validator for <see cref="DeleteParagraphInput"/>
    /// </summary>
    public class DeleteParagraphValidator : AbstractValidator<DeleteParagraphInput>
    {
        /// <inheritdoc />
        public DeleteParagraphValidator()
        {
            RuleFor(r => r.ParaId)
                .NotNull()
                .Must(paraId => paraId.IsEmpty == false);
        }
    }
}
