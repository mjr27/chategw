using AngleSharp.Text;

using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Persistence;

using FluentValidation;
namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Validator for <see cref="SetPublicationMetadataInput"/>
/// </summary>
public class SetPublicationMetadataValidator : AbstractValidator<SetPublicationMetadataInput>
{

    /// <inheritdoc />
    public SetPublicationMetadataValidator(PublicationDbContext dbContext)
    {
        RuleFor(r => r.Code)
            .Length(2, 7)
            .Matches(@"[a-z0-9]")
            .Must(code => code[0].IsUppercaseAscii());

        RuleFor(r => r.Title)
            .NotEmpty()
            .Must(title => title.Length == title.Trim().Length)
            .MaximumLength(400);

        RuleFor(r => r.AuthorId)
            .MustAsync(dbContext.AuthorExist).When(r => r.AuthorId is not null);

        RuleFor(r => r.Publisher)
            .MaximumLength(400);

        RuleFor(r => r.PageCount)
            .GreaterThan(0).When(r => r.PageCount is not null);

        RuleFor(r => r.PublicationYear)
            .Must(publicationYear => publicationYear > 1000 && publicationYear <= DateTime.Now.Year + 2);

        RuleFor(r => r.Isbn)
            .Must(isbn => ISBN.TryParse(isbn, out _));
    }
}