using AngleSharp.Html.Parser;

using Egw.PubManagement.Application.Messaging.Import;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Serialization;

namespace Egw.PubManagement.Application.Messaging.Archive;

/// <inheritdoc />
public class RestoreEntryFromArchiveHandler : ApplicationCommandHandler<RestoreEntryFromArchiveInput>
{
    private readonly IBackgroundTaskQueue _queue;

    /// <inheritdoc />
    public override async Task Handle(RestoreEntryFromArchiveInput request, CancellationToken cancellationToken)
    {
        bool versionExists = await this._db.PublicationArchive
            .AnyAsync(r => r.PublicationId == request.PublicationId && r.Id == request.Id && r.DeletedAt == null,
                cancellationToken);
        if (!versionExists)
        {
            throw new NotFoundProblemDetailsException("Archive entry not found");
        }

        _queue.AddTask(Guid.NewGuid(),
            $"Restoring archived publication {request.PublicationId} from version {request.Id} ",
            MakeTask(request));
    }

    /// <inheritdoc />
    public RestoreEntryFromArchiveHandler(PublicationDbContext db, IClock clock, IBackgroundTaskQueue queue) : base(db,
        clock)
    {
        _queue = queue;
    }

    private static Func<IServiceProvider, CancellationToken, Task> MakeTask(RestoreEntryFromArchiveInput request)
    {
        return async (sp, cancellationToken) =>
        {
            using IServiceScope scope = sp.CreateScope();
            PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var archivedEntity = await db.PublicationArchive
                .AsNoTracking()
                .Where(r => r.Id == request.Id && r.DeletedAt == null)
                .Select(r => new { r.ArchiveText })
                .FirstOrDefaultAsync(cancellationToken);
            if (archivedEntity is null)
            {
                throw new NotFoundProblemDetailsException("Archive entry not found");
            }

            DateTimeOffset lastArchivedAt = await db.PublicationArchive
                .Where(r => r.PublicationId == request.PublicationId && r.DeletedAt == null)
                .MaxAsync(r => r.ArchivedAt, cancellationToken);
            DateTimeOffset maxParagraphModifiedAt = await db.Paragraphs
                .Where(r => r.PublicationId == request.PublicationId)
                .Select(r => r.UpdatedAt)
                .DefaultIfEmpty(DateTimeOffset.MinValue)
                .MaxAsync(cancellationToken);
            DateTimeOffset publicationModifiedAt = await db.Publications
                .Where(r => r.PublicationId == request.PublicationId)
                .Select(r => r.UpdatedAt)
                .SingleAsync(cancellationToken);
            if (lastArchivedAt < maxParagraphModifiedAt && lastArchivedAt < publicationModifiedAt)
            {
                if (!request.AutoBackup)
                {
                    throw new ConflictProblemDetailsException("Please archive the latest version first");
                }

                await mediator.Send(
                    new SavePublicationToArchiveInput { PublicationId = request.PublicationId },
                    cancellationToken);
            }

            var deserializer = new WemlDeserializer();
            var parser = new HtmlParser();
            WemlDocument deserialized =
                deserializer.Deserialize(await parser.ParseDocumentAsync(archivedEntity.ArchiveText,
                    cancellationToken));
            await mediator.Send(new LoadDocumentCommand(deserialized, true, false), cancellationToken);
        };
    }
}