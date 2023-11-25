using System.Runtime.CompilerServices;

using Egw.PubManagement.LatexExport;
using Egw.PubManagement.LatexExport.Models;
using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Services.Latex;

/// <summary>
/// Publication reader from the database
/// </summary>
public class DatabaseLatexPublicationRepository : ILatexPublicationRepository
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="dbContextFactory">Publication database context factory</param>
    public DatabaseLatexPublicationRepository(IDbContextFactory<PublicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc />
    public async Task<LatexPublicationDto?> GetPublication(int publicationId, CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var publication = await db.Publications
            .Include(r => r.Author)
            .Where(r => r.PublicationId == publicationId)
            .Select(r => new
            {
                r.PublicationId,
                r.Author,
                r.Title,
                r.LanguageCode,
                r.Type,
                r.PublicationYear
            }).FirstOrDefaultAsync(cancellationToken);
        if (publication is null)
        {
            return null;
        }


        return new LatexPublicationDto
        {
            PublicationId = publication.PublicationId,
            Author = publication.Author?.FullName,
            Title = publication.Title,
            Type = publication.Type,
            LanguageCode = publication.LanguageCode,
            PublicationYear = publication.PublicationYear
        };
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<LatexParagraphDto> GetParagraphs(
        int publicationId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IAsyncEnumerable<LatexParagraphDto> paragraphs = db.Paragraphs
            .Where(r => r.PublicationId == publicationId)
            .OrderBy(r => r.Order)
            .Select(r => new LatexParagraphDto
            {
                WemlContent = r.Content, HeadingLevel = r.HeadingLevel, ParagraphId = r.ParagraphId
            })
            .AsAsyncEnumerable();
        await foreach (LatexParagraphDto item in paragraphs)
        {
            yield return item;
        }
    }
}