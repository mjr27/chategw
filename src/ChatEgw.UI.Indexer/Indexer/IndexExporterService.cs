using ChatEgw.UI.Indexer.Indexer.Generators;
using ChatEgw.UI.Persistence;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using EnumsNET;
using Microsoft.EntityFrameworkCore;
using ShellProgressBar;
using WhiteEstate.DocFormat.Enums;

namespace ChatEgw.UI.Indexer.Indexer;

public class IndexExporterService
{
    private readonly PublicationDbContext _db;

    public IndexExporterService(PublicationDbContext db)
    {
        _db = db;
    }

    public List<SearchNode> GetNodes()
    {
        int[] egwFolders = _db.Folders.Where(
            r =>
                (r.ParentId == 2
                 || r.Parent!.ParentId == 2
                 || r.Parent!.Parent!.ParentId == 2
                 || r.Parent.Parent.Parent!.ParentId == 2
                 || r.Parent.Parent.Parent.Parent!.ParentId == 2
                 || r.Parent.Parent.Parent.Parent!.Parent!.ParentId == 2
                 | r.Parent.Parent.Parent.Parent!.Parent!.Parent!.ParentId == 2)
                && r.Id != 218
        ).Select(r => r.Id).ToArray();
        PublicationType[] disallowedTypes = { PublicationType.TopicalIndex,
            PublicationType.ScriptureIndex };
        var allPublications = _db.Publications
            // .Where(r=>r.PublicationId == 127 ||r.PublicationId == 1965)
            .Where(r => r.LanguageCode == "eng")
            .Where(r => !disallowedTypes.Contains(r.Type))
            .Where(r => r.Type != PublicationType.Bible || r.PublicationId == 1965)
            .Include(r => r.Placement)
            .Where(r => r.Placement != null)
            .ToList();


        int[] egwPublications = allPublications
            .Where(
                r => (r.AuthorId is 1 or 187 && r.Placement!.FolderId == 1371)
                     || egwFolders.Contains(r.Placement!.FolderId)
            ).Select(r => r.PublicationId).ToArray();
        var allFolders = _db.Folders.ToList();
        int[] distinctFolders = allPublications.Select(r => r.Placement!.FolderId).Distinct().ToArray();
        IEnumerable<Folder> publicationsFolders = FilterFolders(allFolders, distinctFolders);

        var newFolders = publicationsFolders.OrderBy(r => r.GlobalOrder)
            .Select(f => new
            {
                Folder = f,
                Children = allPublications.Where(r => r.Placement!.FolderId == f.Id).ToList()
            })
            .ToList();
        var nodes = new List<SearchNode>
        {
            new SearchNode
            {
                Id = "f1",
                Title = "EGW Writings",
                IsEgw = false,
                GlobalOrder = 0,
                Date = null,
                IsFolder = true,
                ParentId = null
            }
        };
        var i = 1;
        foreach (var f in newFolders.Where(r => r.Folder.ParentId != null))
        {
            nodes.Add(new SearchNode
            {
                Id = $"f{f.Folder.Id}",
                Title = f.Folder.Title,
                IsEgw = false,
                GlobalOrder = i++,
                Date = null,
                IsFolder = true,
                ParentId = $"f{f.Folder.ParentId}",
            });
            nodes.AddRange(f.Children.Select(child => new SearchNode
            {
                Id = $"p{child.PublicationId}",
                Title = child.Title,
                GlobalOrder = i++,
                IsEgw = egwPublications.Contains(child.PublicationId),
                IsFolder = false,
                Date = child.PublicationYear is null ? null : new DateOnly(child.PublicationYear.Value, 1, 1),
                ParentId = $"f{f.Folder.Id}",
            }));
        }

        CalculateChildren(nodes);

        return nodes;
    }


    public IEnumerable<SearchParagraph> FetchParagraphs(Dictionary<int, bool> egwPublicationStatus)
    {
        using var bar = new ProgressBar(egwPublicationStatus.Count, "Converting publications");
        var publications = _db.Publications
            .Where(r => egwPublicationStatus.Select(p => p.Key).Contains(r.PublicationId))
            .Select(r => new IndexPublicationDto(
                r.PublicationId,
                r.Code,
                r.Title,
                r.Type,
                r.PublicationYear,
                egwPublicationStatus[r.PublicationId],
                _db.PublicationSynonyms.Where(p => r.PublicationId == p.PublicationId && p.ElementId == null)
                    .Select(p => p.Synonym)
                    .ToArray()
            )).ToList();
        foreach (IndexPublicationDto publication in publications)
        {
            BaseParagraphGenerator handler = publication.Type switch
            {
                PublicationType.Bible => new BibleParagraphGenerator(_db, publication, 3),
                PublicationType.Dictionary => new BibleParagraphGenerator(_db, publication, 4),
                PublicationType.Book or PublicationType.Devotional or PublicationType.BibleCommentary =>
                    new BookParagraphGenerator(_db, publication),
                PublicationType.PeriodicalPageBreak or PublicationType.PeriodicalNoPageBreak => new
                    BookParagraphGenerator(
                        _db, publication
                    ),
                PublicationType.Manuscript => new BookParagraphGenerator(_db, publication),
                _ => throw new ArgumentException(
                    "Unknown type:" + publication.Type.AsString(EnumFormat.Description)
                )
            };

            foreach (SearchParagraph para in handler.Index())
            {
                yield return para;
            }

            bar.Tick();
        }
    }

    private static IEnumerable<Folder> FilterFolders(IReadOnlyCollection<Folder> allFolders, int[] distinctFolders)
    {
        var publicationsFolders = allFolders
            .Where(f => distinctFolders.Contains(f.Id)).ToList();
        while (true)
        {
            var currentParentFolders = publicationsFolders.Select(r => r.ParentId).ToHashSet();
            Folder[] parentFolders = allFolders.Where(r => currentParentFolders.Contains(r.Id)
                                                           && publicationsFolders.All(f => f.Id != r.Id))
                .ToArray();
            if (parentFolders.Length == 0)
            {
                break;
            }

            publicationsFolders.AddRange(parentFolders);
        }

        return publicationsFolders;
    }

    private static void CalculateChildren(List<SearchNode> nodes)
    {
        foreach (SearchNode node in nodes)
        {
            var children = new List<string>();
            SearchNode? current = node;
            do
            {
                children.Add(current.Id);
                current = nodes.FirstOrDefault(r => r.Id == current.ParentId);
            } while (current != null);

            node.Children = children.ToArray();
        }
    }
}