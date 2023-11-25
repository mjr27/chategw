using FluentValidation;

namespace Egw.PubManagement.Application.Messaging.Archive;

/// <inheritdoc />
public class SavePublicationToArchiveValidator : AbstractValidator<SavePublicationToArchiveInput>
{
    /// <inheritdoc />
    public SavePublicationToArchiveValidator()
    {
        RuleFor(r => r.PublicationId)
            .GreaterThan(0);
    }
}