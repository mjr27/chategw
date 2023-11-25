using System.ComponentModel.DataAnnotations;

using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Services;
using Egw.PubManagement.Preview.Models;
using Egw.PubManagement.Preview.Models.Internal;
using Egw.PubManagement.Preview.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Preview.Controllers;

/// <summary>
/// Publication exports
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]/books")]
public class ContentController : ControllerBase
{
    private readonly PublicationDbContext _db;
    private readonly AccessiblePublicationsService _accessiblePublications;
    private readonly ParagraphConverterService _converter;
    private readonly ParagraphCacheService _paragraphCache;

    /// <inheritdoc />
    public ContentController(PublicationDbContext db,
        AccessiblePublicationsService accessiblePublications,
        ParagraphCacheService paragraphCache,
        ParagraphConverterService converter)
    {
        _db = db;
        _accessiblePublications = accessiblePublications;
        _converter = converter;
        _paragraphCache = paragraphCache;
    }

    private static BookType ConvertType(PublicationType t) => t switch
    {
        PublicationType.Bible => BookType.Bible,
        PublicationType.Book => BookType.Book,
        PublicationType.Devotional => BookType.Book,
        PublicationType.BibleCommentary => BookType.Book,
        PublicationType.PeriodicalPageBreak => BookType.Periodical,
        PublicationType.PeriodicalNoPageBreak => BookType.Periodical,
        PublicationType.Manuscript => BookType.LetterOrManuscript,
        PublicationType.Dictionary => BookType.Dictionary,
        PublicationType.TopicalIndex => BookType.TopicalIndex,
        PublicationType.ScriptureIndex => BookType.ScriptureIndex,
        _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
    };

    private static string ConvertSubType(PublicationType t) => t switch
    {
        PublicationType.Devotional => "devotional",
        PublicationType.BibleCommentary => "commentary",
        PublicationType.PeriodicalNoPageBreak => "no pagebreaks",
        _ => ""
    };

    private IQueryable<BookDto> Project(IQueryable<Publication> publications) =>
        publications
            .Include(r => r.Language)
            .Include(r => r.Placement)
            .OrderBy(r => r.Placement!.Folder.GlobalOrder)
            .ThenBy(r => r.Placement!.Order)
            .Select(r => new BookDto
            {
                BookId = r.PublicationId,
                Author = r.Author == null ? "" : r.Author.FullName,
                FirstPara = r.Paragraphs.OrderBy(p => p.Order).First().ParaId,
                Code = r.Code,
                Title = r.Title,
                Description = r.Description,
                Isbn = r.Isbn,
                Language = r.Language.EgwCode,
                Publisher = r.Publisher,
                LastModified = r.UpdatedAt.Date,
                PageCount = r.PageCount ?? 0,
                PubYear = r.PublicationYear == null ? "" : r.PublicationYear.Value.ToString(),
                Sort = r.Placement == null
                    ? 0
                    : r.Placement.Order + r.Placement.Folder.GlobalOrder * 10_000,
                Subtype = ConvertSubType(r.Type),
                Type = ConvertType(r.Type)
            });


    /// <summary>
    /// List books 
    /// </summary>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    public async Task<ResultWrapper<BookDto>> ListBooks(CancellationToken cancellationToken)
    {
        int[] allowedPublications = await _accessiblePublications.GetAccessiblePublications(cancellationToken);
        List<BookDto> data = await Project(_db.Publications
                .OrderBy(r => r.Placement == null ? int.MaxValue : r.Placement.Folder.GlobalOrder)
                .ThenBy(r => r.Placement == null ? int.MaxValue : r.Placement.Order)
                .Include(r => r.Language)
                .Where(r => allowedPublications.Contains(r.PublicationId)))
            .ToListAsync(cancellationToken);
        return new ResultWrapper<BookDto>(data);
    }

    /// <summary>
    /// Get single book
    /// </summary>
    /// <param name="id">Book id</param>
    /// <param name="cancellationToken"></param>
    [HttpGet("{id:int}")]
    public async Task<BookDto> GetBook([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _accessiblePublications.EnsureAccessible(id, cancellationToken);
        int[] allowedPublications = await _accessiblePublications.GetAccessiblePublications(cancellationToken);
        if (!allowedPublications.Contains(id))
        {
            throw new NotFoundProblemDetailsException();
        }

        return await Project(_db.Publications
                       .Where(r => r.PublicationId == id))
                   .FirstOrDefaultAsync(cancellationToken)
               ?? throw new NotFoundProblemDetailsException();
    }

    /// <summary>
    /// Get book TOC
    /// </summary>
    /// <param name="id">Book id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:int}/toc")]
    // [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/html")]
    public async Task<List<TocDto>> GetToc([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _accessiblePublications.EnsureAccessible(id, cancellationToken);
        Publication publication = await _db.Publications
                                      .Where(r => r.PublicationId == id)
                                      .FirstOrDefaultAsync(cancellationToken)
                                  ?? throw new NotFoundProblemDetailsException();
        var data = await _db.PublicationChapters
            .Where(r => r.PublicationId == id)
            .Include(r => r.Paragraph)
            .OrderBy(r => r.Order)
            .Select(r => new
            {
                Chapter = r.ChapterId,
                Toc = new TocDto
                {
                    Level = r.Level - 1,
                    Title = r.Title,
                    Mp3Path = null,
                    ParaId = r.ParaId,
                    PubOrder = r.Order,
                    IsDuplicateOf = null,
                    RefCodeShort = r.Paragraph.Metadata == null
                        ? ""
                        : IRefCodeGenerator.Instance.Short(publication.Type, publication.Code, r.Paragraph.Metadata) ?? ""
                }
            }).ToListAsync(cancellationToken);
        var lastChapter = new ParaId();
        var lastItem = new ParaId();

        foreach (var item in data)
        {
            if (item.Chapter == lastChapter)
            {
                item.Toc.IsDuplicateOf = lastItem;
            }

            lastChapter = item.Chapter;
            lastItem = item.Toc.ParaId;
        }

        return data.Select(r => r.Toc).ToList();
    }

    /// <summary>
    /// Get single book chapter
    /// </summary>
    /// <param name="id">Book id</param>
    /// <param name="chapterId">Chapter id</param>
    /// <param name="cancellationToken"></param>
    [HttpGet("{id:int}/chapter/{chapterId:int}")]
    // [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/html")]
    public async Task<List<LegacyParagraphDto>> GetBookChapter([FromRoute] int id, [FromRoute] int chapterId,
        CancellationToken cancellationToken)
    {
        await _accessiblePublications.EnsureAccessible(id, cancellationToken);
        var paraId = new ParaId(id, chapterId);
        ParaOrderDto para = await _paragraphCache.GetOrder(paraId, cancellationToken);
        if (para.ParaId.IsEmpty)
        {
            throw new NotFoundProblemDetailsException();
        }

        Range chapterRange = await _paragraphCache.GetChapterOrderRange(id, para.Order, cancellationToken)
                             ?? throw new NotFoundProblemDetailsException();
        List<ParagraphTemporaryDto> paragraphs = await _converter.Project(_db.Paragraphs
                .OrderBy(r => r.Order)
                .Where(r => r.PublicationId == id && r.Order >= chapterRange.Start.Value && r.Order <= chapterRange.End.Value)
                .Where(r => r.HeadingLevel != null))
            .ToListAsync(cancellationToken);
        var result = paragraphs.Select(r => _converter.Convert(r)).ToList();
        await _paragraphCache.FillParagraphOrder(result, cancellationToken);
        return result;
    }

    /// <summary>
    /// Get chunk of content
    /// </summary>
    /// <param name="id">Book id</param>
    /// <param name="elementId">Element id</param>
    /// <param name="direction">Content direction</param>
    /// <param name="cancellationToken"></param>
    /// <param name="limit">Limit of items to retrieve</param>
    [HttpGet("{id:int}/content/{elementId:int}")]
    // [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/html")]
    public async Task<List<LegacyParagraphDto>> GetBookContent([FromRoute] int id, [FromRoute] int elementId,
        CancellationToken cancellationToken,
        [Range(minimum: 1, maximum: 200)] [FromQuery]
        int limit = 1,
        [FromQuery] ContentDirection direction = ContentDirection.Down
    )
    {
        await _accessiblePublications.EnsureAccessible(id, cancellationToken);
        var paraId = new ParaId(id, elementId);
        ParaOrderDto para = await _paragraphCache.GetOrder(paraId, cancellationToken);
        if (para.ParaId.IsEmpty)
        {
            throw new NotFoundProblemDetailsException();
        }

        List<ParagraphTemporaryDto> paragraphs;

        switch (direction)
        {
            case ContentDirection.Down:
                paragraphs = await _converter.Project(_db.Paragraphs
                        .OrderBy(r => r.Order)
                        .Where(r => r.PublicationId == id && r.Order >= para.Order)
                        .Where(r => r.HeadingLevel != null))
                    .Take(limit)
                    .ToListAsync(cancellationToken);
                break;

            case ContentDirection.Up:
                paragraphs = await _converter.Project(_db.Paragraphs
                        .OrderByDescending(r => r.Order)
                        .Where(r => r.PublicationId == id && r.Order <= para.Order)
                        .Where(r => r.HeadingLevel != null))
                    .Take(limit)
                    .ToListAsync(cancellationToken);
                paragraphs.Reverse();
                break;

            case ContentDirection.Both:
                paragraphs = await _converter.Project(_db.Paragraphs
                        .OrderByDescending(r => r.Order)
                        .Where(r => r.PublicationId == id && r.Order < para.Order)
                        .Where(r => r.HeadingLevel != null))
                    .Take(limit)
                    .ToListAsync(cancellationToken);
                paragraphs.Reverse();


                paragraphs.AddRange(await _converter.Project(_db.Paragraphs
                        .OrderBy(r => r.Order)
                        .Where(r => r.PublicationId == id && r.Order >= para.Order)
                        .Where(r => r.HeadingLevel != null))
                    .Take(limit + 1)
                    .ToListAsync(cancellationToken));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        var result = paragraphs.Select(_converter.Convert).ToList();
        await _paragraphCache.FillParagraphOrder(result, cancellationToken);
        return result;
    }

}