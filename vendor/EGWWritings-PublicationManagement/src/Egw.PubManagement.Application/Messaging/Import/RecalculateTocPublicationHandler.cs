using System.Text.RegularExpressions;

using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Messaging.Import;

/// <inheritdoc />
public partial class RecalculateTocPublicationHandler : IRequestHandler<RecalculateTocPublicationCommand>
{
    private readonly PublicationDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// Command handler
    /// </summary>
    public RecalculateTocPublicationHandler(PublicationDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private class ChapterLevel
    {
        public string Title { get; set; } = "";
        public int Level { get; set; }
        public ParaId StartPara { get; set; }
        public int Order { get; set; }
        public ParaId EndPara { get; set; }
        public int EndOrder { get; set; }
        public ParaId ContentEndPara { get; set; }
        public int ContentEndOrder { get; set; }
        public int Chapter { get; set; }
    }


    /// <inheritdoc />
    public async Task Handle(RecalculateTocPublicationCommand request, CancellationToken cancellationToken)
    {
        Publication publication = await _db.Publications
                                      .AsNoTracking()
                                      .Include(r => r.Placement)
                                      .FirstOrDefaultAsync(p => p.PublicationId == request.PublicationId, cancellationToken)
                                  ?? throw new NotFoundProblemDetailsException("Publication not found");

        List<Paragraph> paras = await _db.Paragraphs
            .AsNoTracking()
            .Where(r => r.PublicationId == request.PublicationId)
            .OrderBy(r => r.Order)
            .ToListAsync(cancellationToken);
        paras = paras
            .ToList();
        if (!paras.Any())
        {
            return;
        }

        await _db.PublicationChapters.Where(r => r.PublicationId == request.PublicationId)
            .ExecuteDeleteAsync(cancellationToken);

        IEnumerable<ChapterLevel> chapterList = ExtractToc(publication,
            paras,
            publication.Placement?.TocDepth ?? DbExtensions.GetDefaultHeadingLevel(publication.Type)
        );

        var tableOfContents = new List<PublicationChapter>();
        foreach (ChapterLevel? tocEntry in chapterList)
        {
            var chapter = new PublicationChapter(
                tocEntry.StartPara,
                tocEntry.Order,
                tocEntry.EndPara,
                tocEntry.EndOrder,
                tocEntry.ContentEndPara,
                tocEntry.ContentEndOrder,
                tocEntry.Level,
                new ParaId(request.PublicationId, tocEntry.Chapter),
                tocEntry.Title, _clock.Now);
            tableOfContents.Add(chapter);
        }

        _db.PublicationChapters.AddRange(tableOfContents);
        await _db.SaveChangesAsync(cancellationToken);
        _db.ChangeTracker.Clear();
    }

    private static IEnumerable<ChapterLevel> ExtractToc(Publication publication, IReadOnlyList<Paragraph> paras,
        int maxLevel)
    {
        int prefixCount = paras.TakeWhile(r => r.HeadingLevel is null or 0).Count() + 1;
        Paragraph[] indexPrefix = paras.Take(prefixCount).ToArray();
        ChapterLevel root = CreatChapterLevel(publication, paras[0]);
        root.Level = 1;
        root.EndOrder = indexPrefix[^1].Order;
        root.ContentEndOrder = indexPrefix[^1].Order;
        root.EndPara = indexPrefix[^1].ParaId;
        root.ContentEndPara = indexPrefix[^1].ParaId;
        root.Title = ExtractTitle(publication, indexPrefix[^1]);

        var dataStack = new List<ChapterLevel> { root };
        var result = new List<ChapterLevel>();
        foreach (Paragraph? para in paras.Skip(prefixCount))
        {
            if (!(para.HeadingLevel >= 1 && para.HeadingLevel <= maxLevel))
            {
                foreach (ChapterLevel? item in dataStack)
                {
                    item.EndPara = para.ParaId;
                    item.EndOrder = para.Order;
                }

                dataStack[^1].ContentEndPara = para.ParaId;
                dataStack[^1].ContentEndOrder = para.Order;
                continue;
            }

            ChapterLevel chapter = CreatChapterLevel(publication, para);
            while (chapter.Level <= dataStack[^1].Level)
            {
                result.Add(dataStack[^1]);
                dataStack.RemoveAt(dataStack.Count - 1);
            }

            dataStack.Add(chapter);
        }

        result.AddRange(dataStack);
        result = result
            .OrderBy(r => r.Order)
            .ToList();
        FixChapterLevels(result);
        EnrichWithContinuousChapters(result, paras);
        return result;
    }

    private static void EnrichWithContinuousChapters(IReadOnlyList<ChapterLevel> chapterList,
        IReadOnlyCollection<Paragraph> paragraphs)
    {
        if (chapterList.Count == 1)
        {
            return;
        }

        for (int i = 1; i < chapterList.Count; i++)
        {
            ChapterLevel previousChapter = chapterList[i - 1];
            ChapterLevel chapter = chapterList[i];

            if (IsEmptyChapter(previousChapter, paragraphs))
            {
                chapter.Chapter = previousChapter.Chapter;
            }
        }

        IEnumerable<IGrouping<int, ChapterLevel>> groupedChapters = chapterList.GroupBy(r => r.Chapter);
        foreach (IGrouping<int, ChapterLevel>? chapterGroup in groupedChapters)
        {
            ChapterLevel[] chapters = chapterGroup.ToArray();
            int minOrder = chapters.Min(r => r.Order);
            int maxOrder = chapters.Max(r => r.ContentEndOrder);
            int firstHeaderId = paragraphs
                .Where(r => r.Order >= minOrder && r.Order <= maxOrder)
                .OrderBy(r => r.Order)
                .First(r => r is { HeadingLevel: > 0 })
                .ParagraphId;
            foreach (ChapterLevel? chapter in chapters)
            {
                chapter.Chapter = firstHeaderId;
            }
        }
    }

    private static bool IsEmptyChapter(ChapterLevel chapter, IReadOnlyCollection<Paragraph> paragraphs)
    {
        return paragraphs
            .Where(r => r.Order >= chapter.Order && r.Order <= chapter.ContentEndOrder)
            .All(r => r.HeadingLevel != 0);
    }

    private static void FixChapterLevels(IReadOnlyList<ChapterLevel> chapters)
    {
        var levels = new List<int> { 1 };

        for (int i = 1; i < chapters.Count; i++)
        {
            int lastLevel = levels[^1];
            ChapterLevel chapter = chapters[i];
            if (chapter.Level > lastLevel)
            {
                levels.Add(chapter.Level);
                chapter.Level = levels.Count;
                continue;
            }

            if (chapter.Level == lastLevel)
            {
                chapter.Level = levels.Count;
                continue;
            }

            while (chapter.Level <= lastLevel)
            {
                levels.RemoveAt(levels.Count - 1);
                lastLevel = levels[^1];
            }

            levels.Add(chapter.Level);
            chapter.Level = levels.Count;
        }
    }

    private static ChapterLevel CreatChapterLevel(Publication publication, Paragraph paragraph)
    {
        return new ChapterLevel
        {
            StartPara = paragraph.ParaId,
            Order = paragraph.Order,
            EndPara = paragraph.ParaId,
            EndOrder = paragraph.Order,
            ContentEndPara = paragraph.ParaId,
            ContentEndOrder = paragraph.Order,
            Chapter = paragraph.ParaId.ElementId,
            Title = ExtractTitle(publication, paragraph),
            Level = paragraph.HeadingLevel ?? 1
        };
    }

    private static string ExtractTitle(Publication publication, Paragraph paragraph)
    {
        string title = publication.Type switch
        {
            _ => paragraph.DeserializedContent().TextContent()
        };
        return FixTitle(title);
    }

    private static string FixTitle(string title)
    {
        return ReSpaces().Replace(title, " ").Trim();
    }

    [GeneratedRegex("\\s+", RegexOptions.Compiled)]
    private static partial Regex ReSpaces();
}