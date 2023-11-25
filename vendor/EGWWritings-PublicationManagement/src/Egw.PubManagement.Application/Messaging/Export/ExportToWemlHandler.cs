using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Messaging.Export;

/// <inheritdoc />
public class ExportToWemlHandler : IRequestHandler<ExportToWemlQuery, WemlDocument>
{
    private readonly PublicationDbContext _db;

    /// <summary>
    /// Command handler
    /// </summary>
    public ExportToWemlHandler(PublicationDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<WemlDocument> Handle(ExportToWemlQuery request, CancellationToken cancellationToken)
    {
        Publication? publication = await _db.Publications
            .Where(r => r.PublicationId == request.PublicationId)
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
        foreach (Paragraph? paragraph in _db.Paragraphs
                     .AsNoTracking()
                     .Where(r => r.PublicationId == request.PublicationId)
                     .OrderBy(r => r.Order))
        {
            document.Children.Add(new WemlParagraph(paragraph.ParagraphId, paragraph.DeserializedContent()));
        }

        return document;
    }
}