using Egw.PubManagement.Persistence;

using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Covers;

/// <inheritdoc />
public class DeleteCoverTypeValidator : AbstractValidator<DeleteCoverTypeInput>
{
    /// <summary>
    /// 
    /// </summary>
    public DeleteCoverTypeValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(PublicationDbContext.CoverTypeLength);
    }
}