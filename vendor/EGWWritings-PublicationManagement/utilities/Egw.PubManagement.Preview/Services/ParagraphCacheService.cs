using Egw.PubManagement.Persistence;
using Egw.PubManagement.Preview.Models;
using Egw.PubManagement.Preview.Models.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Preview.Services;

/// <summary>
/// Paragraph Cache Service
/// </summary>
public class ParagraphCacheService
{
    private readonly PublicationDbContext _db;
    private readonly IMemoryCache _memoryCache;

    /// <summary> Default constructor </summary>
    public ParagraphCacheService(PublicationDbContext db, IMemoryCache memoryCache)
    {
        _db = db;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Retrieves publication order
    /// </summary>
    /// <param name="paraId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ParaOrderDto> GetOrder(ParaId paraId, CancellationToken cancellationToken)
    {
        ParaOrderDto[] book = await LoadBook(paraId.PublicationId, cancellationToken);
        ParaOrderDto value = book.FirstOrDefault(r => r.ParaId == paraId);
        return value.ParaId == paraId ? value : book.FirstOrDefault();
    }

    /// <summary>
    /// Return chapter order range
    /// </summary>
    /// <param name="publicationId"></param>
    /// <param name="order"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Range?> GetChapterOrderRange(int publicationId, int order, CancellationToken cancellationToken)
    {
        ChapterOrderDto[] chapters = await LoadChapters(publicationId, cancellationToken);
        ChapterOrderDto value = chapters.FirstOrDefault(r => r.Order <= order && r.EndOrder >= order);
        if (value.ParaId.IsEmpty)
        {
            return null;
        }

        IEnumerable<ChapterOrderDto> allChapters = chapters.Where(r => r.ChapterId == value.ChapterId).ToList();
        return new Range(allChapters.Min(r => r.Order), allChapters.Max(r => r.EndOrder));
    }

    private async Task<ParaOrderDto[]> LoadBook(int bookId, CancellationToken cancellationToken)
    {
        string key = $"publication-{bookId}";
        if (_memoryCache.TryGetValue(key, out ParaOrderDto[]? value) && value is not null)
        {
            return value;
        }

        ParaOrderDto[] publicationCache = await _db.Paragraphs
            .Where(p => p.PublicationId == bookId && p.HeadingLevel != null)
            .OrderBy(r => r.Order)
            .Select(r => new ParaOrderDto(r.ParaId, r.Order))
            .ToArrayAsync(cancellationToken);
        _memoryCache.Set(key, publicationCache, TimeSpan.FromMinutes(5));
        return publicationCache;
    }

    private async Task<ChapterOrderDto[]> LoadChapters(int bookId, CancellationToken cancellationToken)
    {
        string key = $"chapters-{bookId}";
        if (_memoryCache.TryGetValue(key, out ChapterOrderDto[]? value) && value is not null)
        {
            return value;
        }

        ChapterOrderDto[] publicationCache = await _db.PublicationChapters
            .Where(p => p.PublicationId == bookId)
            .OrderBy(r => r.Order)
            .Select(r => new ChapterOrderDto(r.ParaId, r.ChapterId, r.Order, r.ContentEndOrder))
            .ToArrayAsync(cancellationToken);
        _memoryCache.Set(key, publicationCache, TimeSpan.FromMinutes(5));
        return publicationCache;
    }


    /// <summary>
    /// Fills paragraph orders
    /// </summary>
    /// <param name="result"></param>
    /// <param name="cancellationToken"></param>
    public async Task FillParagraphOrder(List<LegacyParagraphDto> result, CancellationToken cancellationToken)
    {
        if (result.Count == 0)
        {
            return;
        }

        if (result.Count > 1)
        {
            for (int i = 1; i < result.Count; i++)
            {
                result[i - 1].NextParaId = result[i].ParaId;
                result[i].PreviousParaId = result[i - 1].ParaId;
            }
        }

        ParaOrderDto[] orders = await this.LoadBook(result[0].ParaId.PublicationId, cancellationToken);
        
        LegacyParagraphDto item = result[0];
        int index = Array.FindIndex(orders, r => r.ParaId == item.ParaId);
        if (index > 0)
        {
            item.PreviousParaId = orders[index - 1].ParaId;
        }

        item = result[^1];
        index = Array.FindIndex(orders, r => r.ParaId == item.ParaId);
        if (index < orders.Length - 2)
        {
            item.NextParaId = orders[index + 1].ParaId;
        }
    }
}