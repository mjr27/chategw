using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Application.Messaging.Languages;

/// <inheritdoc />
public class UpdateLanguageHandler : ApplicationCommandHandler<UpdateLanguageInput>
{
    /// <inheritdoc />
    public override async Task Handle(UpdateLanguageInput request, CancellationToken cancellationToken)
    {
        Language language = await _db.Languages.FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken)
                            ?? throw new NotFoundProblemDetailsException("Language not found");
        if (!string.IsNullOrWhiteSpace(request.EgwCode))
        {
            language.SetEgwCode(request.EgwCode, Now);
        }

        if (!string.IsNullOrWhiteSpace(request.Bcp47Code))
        {
            language.SetBcp47Code(request.Bcp47Code, Now);
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            language.SetTitle(request.Title, Now);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public UpdateLanguageHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}