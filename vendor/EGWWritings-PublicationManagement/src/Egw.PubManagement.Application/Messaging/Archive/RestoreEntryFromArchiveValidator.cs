using FluentValidation;

namespace Egw.PubManagement.Application.Messaging.Archive;

/// <inheritdoc />
public class RestoreEntryFromArchiveValidator : AbstractValidator<RestoreEntryFromArchiveInput>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RestoreEntryFromArchiveValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PublicationId).NotEmpty().GreaterThan(0);
    }
}