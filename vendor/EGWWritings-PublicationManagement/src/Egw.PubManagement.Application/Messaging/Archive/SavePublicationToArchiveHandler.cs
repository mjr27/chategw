using System.Security.Cryptography;
using System.Text;

using AngleSharp;

using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Serialization;

using IServiceProvider = System.IServiceProvider;

namespace Egw.PubManagement.Application.Messaging.Archive;

/// <inheritdoc />
public class SavePublicationToArchiveHandler : IRequestHandler<SavePublicationToArchiveInput>
{
    private readonly IBackgroundTaskQueue _queue;

    /// <inheritdoc />
    public Task Handle(SavePublicationToArchiveInput request, CancellationToken cancellationToken)
    {
        _queue.AddTask(Guid.NewGuid(), $"Archiving publication # {request.PublicationId}",
            MakeTask(request.PublicationId));
        return Task.CompletedTask;
    }

    private static Func<IServiceProvider, CancellationToken, Task> MakeTask(int publicationId)
    {
        return async (sp, cancellationToken) =>
        {
            var serializer = new WemlSerializer();

            using IServiceScope scope = sp.CreateScope();
            IClock clock = scope.ServiceProvider.GetRequiredService<IClock>();
            PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();
            Publication? publication = await db.Publications
                .Where(r => r.PublicationId == publicationId)
                .Include(r => r.Language)
                .FirstOrDefaultAsync(cancellationToken);
            if (publication is null)
            {
                throw new NotFoundProblemDetailsException("Publication not found");
            }

            string languageCode = publication.Language.Bcp47Code;

            var header =
                new WemlDocumentHeader(publication.PublicationId, publication.Type, languageCode, publication.Code)
                {
                    Title = publication.Title
                };
            var document = new WemlDocument(header);

            foreach (Paragraph? paragraph in db.Paragraphs
                         .AsNoTracking()
                         .Where(r => r.PublicationId == publicationId)
                         .OrderBy(r => r.Order))
            {
                document.Children.Add(new WemlParagraph(paragraph.ParagraphId, paragraph.DeserializedContent()));
            }

            string html = serializer.Serialize(document).ToHtml(new OutputMarkupFormatter());
            byte[] hashData = SHA256.HashData(Encoding.UTF8.GetBytes(html));
            string hash = Convert.ToHexString(hashData);
            bool entryExists = await db.PublicationArchive
                .Where(r => r.PublicationId == publicationId && r.Hash == hash)
                .AnyAsync(cancellationToken);
            if (entryExists)
            {
                await db.PublicationArchive.Where(r => r.PublicationId == publicationId && r.Hash == hash)
                    .ExecuteUpdateAsync(
                        r => r.SetProperty(e => e.ArchivedAt, clock.Now),
                        cancellationToken);
            }
            else
            {
                var archiveEntry = new ArchivedPublication(publicationId, html, hash, clock.Now);
                db.PublicationArchive.Add(archiveEntry);
                await db.SaveChangesAsync(cancellationToken);
            }
        };
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public SavePublicationToArchiveHandler(IBackgroundTaskQueue queue)
    {
        _queue = queue;
    }
}