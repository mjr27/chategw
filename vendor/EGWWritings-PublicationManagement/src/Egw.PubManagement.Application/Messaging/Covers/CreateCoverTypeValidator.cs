using Egw.PubManagement.Persistence;

using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Covers;

/// <inheritdoc />
public class CreateCoverTypeValidator : AbstractValidator<CreateCoverTypeInput>
{
    /// <inheritdoc />
    public CreateCoverTypeValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(PublicationDbContext.CoverTypeLength);
        RuleFor(x => x.MinWidth).GreaterThan(0);
        RuleFor(x => x.MinHeight).GreaterThan(0);
    }
}