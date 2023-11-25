using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Messaging.Languages;

/// <inheritdoc />
public class CreateLanguageHandler : ApplicationCommandHandler<CreateLanguageInput>
{

    /// <inheritdoc />
    public override async Task Handle(CreateLanguageInput request, CancellationToken cancellationToken)
    {
        var language = new Language(request.Code, request.EgwCode, request.Bcp47Code,
            request.Direction == WemlTextDirection.RightToLeft,
            request.Title,
            Now
        );
        await _db.Languages.AddAsync(language, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public CreateLanguageHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}