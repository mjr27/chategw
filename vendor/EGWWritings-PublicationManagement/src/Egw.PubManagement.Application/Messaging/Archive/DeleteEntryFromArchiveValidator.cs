using FluentValidation;

namespace Egw.PubManagement.Application.Messaging.Archive;

/// <inheritdoc />
public class DeleteEntryFromArchiveValidator : AbstractValidator<DeleteEntryFromArchiveInput>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public DeleteEntryFromArchiveValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PublicationId).NotEmpty().GreaterThan(0);
    }
}