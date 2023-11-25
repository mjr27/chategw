using System.Diagnostics;

using AngleSharp;

using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Serialization;
namespace Egw.PubManagement.Application.Messaging.Import;

/// <inheritdoc />
public class LoadDocumentHandler : IRequestHandler<LoadDocumentCommand>
{
    private readonly PublicationDbContext _db;
    private readonly IClock _clock;
    private readonly ILogger<LoadDocumentHandler> _logger;
    private readonly WemlSerializer _serializer;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Default constructor
    /// </summary>
    public LoadDocumentHandler(
        PublicationDbContext db,
        ILoggerFactory loggerFactory,
        IClock clock
    )
    {
        _db = db;
        _clock = clock;
        _loggerFactory = loggerFactory;
        _serializer = new WemlSerializer();
        _logger = loggerFactory.CreateLogger<LoadDocumentHandler>();
    }

    /// <inheritdoc />
    public async Task Handle(LoadDocumentCommand request, CancellationToken cancellationToken)
    {
        WemlDocument wemlDocument = request.Document;
        DateTimeOffset moment = _clock.Now;
        var sw = Stopwatch.StartNew();
        if (request.SkipExisting)
        {
            bool paragraphsExist = await _db.Paragraphs
                .AnyAsync(r => r.PublicationId == wemlDocument.Header.Id, cancellationToken);
            if (paragraphsExist)
            {
                _logger.LogWarning("Skipping publication {Id} because it contains paragraphs", wemlDocument.Header.Id);
                return;
            }
        }

        await _db.Paragraphs.Where(r => r.PublicationId == wemlDocument.Header.Id)
            .ExecuteDeleteAsync(cancellationToken);
        await _db.PublicationChapters.Where(r => r.PublicationId == wemlDocument.Header.Id)
            .ExecuteDeleteAsync(cancellationToken);
        await _db.ParagraphMetadata.Where(r => r.PublicationId == wemlDocument.Header.Id)
            .ExecuteDeleteAsync(cancellationToken);

        TimeSpan swDelete = sw.Elapsed;
        sw.Restart();
        _logger.LogInformation("Delete took {Elapsed}", swDelete);

        await EnsurePublicationExists(wemlDocument, moment, cancellationToken);

        await _db.ChunkedInsertAsync(
            wemlDocument.Children.Select((r, n) => GetParagraph(r, wemlDocument.Header.Id, n + 1, moment)),
            cancellationToken);

        TimeSpan swInsert = sw.Elapsed;
        sw.Restart();
        _logger.LogInformation("Insert took {Elapsed}", swInsert);

        TimeSpan swMeta = TimeSpan.Zero;
        TimeSpan swToc = TimeSpan.Zero;

        if (request.Recalculate)
        {
            var recalcHandler = new RecalculatePublicationMetadataHandler(_db, _loggerFactory, _clock);
            await recalcHandler.Handle(new RecalculatePublicationMetadataCommand(wemlDocument.Header.Id), cancellationToken);
            swMeta = sw.Elapsed;
            sw.Restart();
            _logger.LogInformation("Metadata took {Elapsed}", swMeta);
            var recalcTocHandler = new RecalculateTocPublicationHandler(_db, _clock);
            await recalcTocHandler.Handle(new RecalculateTocPublicationCommand(wemlDocument.Header.Id), cancellationToken);
            swToc = sw.Elapsed;
            _logger.LogInformation("TOC took {Elapsed}", swToc);
        }

        _logger.LogInformation(
            "Imported {PublicationId} with timing:\n\tDelete: {Delete}\n\tInsert: {Insert}\n\tMeta: {Meta}\n\tTOC: {Toc})",
            wemlDocument.Header.Id,
            swDelete,
            swInsert,
            swMeta,
            swToc
        );
    }

    private async Task EnsurePublicationExists(WemlDocument wemlDocument,
        DateTimeOffset moment,
        CancellationToken cancellationToken)
    {
        string languageCode = await _db.Languages
                                  .AsNoTracking()
                                  .Where(r => r.Code == wemlDocument.Header.Language
                                              || r.EgwCode == wemlDocument.Header.Language
                                              || r.Bcp47Code == wemlDocument.Header.Language)
                                  .Select(r => r.Code)
                                  .FirstOrDefaultAsync(cancellationToken)
                              ?? throw new NotFoundProblemDetailsException(
                                  $"Unknown code: {wemlDocument.Header.Language}");
        Publication? publication = await _db.Publications.Where(r => r.PublicationId == wemlDocument.Header.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (publication is null)
        {
            publication = new Publication(
                wemlDocument.Header.Id,
                wemlDocument.Header.Type,
                languageCode,
                wemlDocument.Header.Code,
                moment).ChangeTitle(wemlDocument.Header.Title, moment);
            _db.Publications.Add(publication);
            return;
        }

        if (publication.Type != wemlDocument.Header.Type)
        {
            throw new ConflictProblemDetailsException(
                $"Invalid publication type `{wemlDocument.Header.Type}` for publication {wemlDocument.Header.Id}"
            );
        }

        if (publication.Code != wemlDocument.Header.Code)
        {
            throw new ConflictProblemDetailsException(
                $"Invalid publication code `{wemlDocument.Header.Code}` for publication {wemlDocument.Header.Id}"
            );
        }

        if (wemlDocument.Header.Title != publication.Title)
        {
            publication.ChangeTitle(wemlDocument.Header.Title, moment);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private Paragraph GetParagraph(WemlParagraph paragraph, int bookId, int order, DateTimeOffset currentDate)
    {
        var element = new Paragraph(new ParaId(bookId, paragraph.Id), currentDate);
        switch (paragraph.Element)
        {
            case WemlHeadingContainer headerContainer:
                element.HeadingLevel = headerContainer.Level;
                element.IsReferenced = !headerContainer.Skip;
                break;
            case WemlParagraphContainer paragraphContainer:
                element.HeadingLevel = 0;
                element.IsReferenced = !paragraphContainer.Skip;
                break;
            case WemlParagraphGroupContainer paragraphContainer:
                element.HeadingLevel = 0;
                element.IsReferenced = !paragraphContainer.Skip;
                break;
            case WemlPageBreakElement:
                element.HeadingLevel = null;
                element.IsReferenced = false;
                break;
            default:
                element.HeadingLevel = null;
                break;
        }

        element.Order = order;
        element.Content = _serializer.Serialize(paragraph.Element).ToHtml();
        return element;
    }
}