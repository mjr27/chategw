using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Languages;

/// <summary>
/// Handler for <see cref="CreateRootFolderForLanguageInput"/>
/// </summary>
public class CreateRootFolderForLanguageHandler : ApplicationCommandHandler<CreateRootFolderForLanguageInput>
{

    /// <inheritdoc />
    public override async Task Handle(CreateRootFolderForLanguageInput request,
        CancellationToken cancellationToken)
    {
        Language? language = await _db.Languages
            .Include(r => r.RootFolder)
            .FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);
        if (language is null)
        {
            throw new NotFoundProblemDetailsException("Language not found");
        }

        if (language.RootFolder is not null)
        {
            throw new ConflictProblemDetailsException("Root folder already exists for this language");
        }

        int maxOrder = await _db.Folders
                           .Where(r => r.ParentId == null)
                           .DefaultIfEmpty()
                           .MaxAsync(r => r == null ? -1 : r.Order, cancellationToken)
                       + 1;
        var folder = new Folder(language.Title, null, maxOrder, "root", Now);
        language.RootFolder = folder;
        await _db.SaveChangesAsync(cancellationToken);
        await _db.RecalculateFolders(Now, cancellationToken);
    }


    /// <inheritdoc />
    public CreateRootFolderForLanguageHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}