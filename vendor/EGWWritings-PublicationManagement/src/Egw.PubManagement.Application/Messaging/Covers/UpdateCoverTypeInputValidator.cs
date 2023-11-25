using Egw.PubManagement.Persistence;

using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Covers;

/// <inheritdoc />
public class UpdateCoverTypeInputValidator : AbstractValidator<UpdateCoverTypeInput>
{
    /// <inheritdoc />
    public UpdateCoverTypeInputValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(PublicationDbContext.CoverTypeLength);
    }
}