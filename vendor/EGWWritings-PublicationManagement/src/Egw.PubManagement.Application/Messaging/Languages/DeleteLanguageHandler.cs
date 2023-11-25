using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Languages;

/// <inheritdoc />
public class DeleteLanguageHandler : ApplicationCommandHandler<DeleteLanguageInput>
{
    /// <inheritdoc />
    public override async Task Handle(DeleteLanguageInput request, CancellationToken cancellationToken)
    {
        Language? language = await _db.Languages.FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);
        if (language is null)
        {
            throw new NotFoundProblemDetailsException($"Language {request.Code} not found");
        }

        if (language.RootFolderId is not null)
        {
            throw new ConflictProblemDetailsException($"Language {request.Code} has folders");
        }

        bool publicationsExist = await _db.Publications.AnyAsync(r => r.LanguageCode == request.Code, cancellationToken);
        if (publicationsExist)
        {
            throw new ConflictProblemDetailsException($"Language {request.Code} has publications");
        }

        _db.Languages.Remove(language);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public DeleteLanguageHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}