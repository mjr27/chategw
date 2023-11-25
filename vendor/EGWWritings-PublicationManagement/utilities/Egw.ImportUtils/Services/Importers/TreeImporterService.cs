using Dapper;

using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.LegacyImport;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using EnumsNET;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using WhiteEstate.DocFormat.Enums;

namespace Egw.ImportUtils.Services.Importers;

public class TreeImporterService : ImporterServiceBase
{
    private readonly ILegacyPublicationTypeDetector _pubTypeDetector;
    private readonly ILogger<TreeImporterService> _logger;
    private readonly IClock _clock;
    private readonly Dictionary<int, PublicationPermissionEnum?> _permissions;

    public TreeImporterService(
        ILegacyPublicationTypeDetector pubTypeDetector,
        IClock clock,
        ILogger<TreeImporterService> logger,
        string publicationDbConnectionString,
        string egwConnectionString,
        Dictionary<int, PublicationPermissionEnum?> permissions
    ) : base(publicationDbConnectionString, egwConnectionString)
    {
        _pubTypeDetector = pubTypeDetector;
        _logger = logger;
        _clock = clock;
        _permissions = permissions;
    }

    public override async Task Import(CancellationToken cancellationToken)
    {
        ClearDatabase();
        DateTimeOffset moment = DateTimeOffset.UtcNow;
        await using IDbContextTransaction t = await _db.Database.BeginTransactionAsync(cancellationToken);
        List<Folder> targetFolders = GetFolders(moment);

        _db.Folders.AddRange(targetFolders);
        _db.Authors.AddRange(GetAuthors(moment));

        IReadOnlyCollection<Language> allLanguages = GetLanguages(targetFolders, moment);
        _db.Languages.AddRange(allLanguages);
        await _db.SaveChangesAsync(cancellationToken);

        var existingRootFolders = allLanguages.Where(r => r.RootFolderId.HasValue)
            .Select(r => r.RootFolderId!.Value)
            .ToHashSet();
        await _db.Folders.Where(r => r.ParentId == null && !existingRootFolders.Contains(r.Id))
            .ExecuteDeleteAsync(cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        IReadOnlyCollection<PublicationPlacement> placements = GetPlacements(moment);

        _db.PublicationPlacement.AddRange(placements);
        var existingPublications = placements.Select(r => r.PublicationId).ToHashSet();

        List<Publication> publications = GetPublications(existingPublications, allLanguages, moment);
        _logger.LogInformation("Publications: {Count}", publications.Count);
        _db.Publications.AddRange(publications);
        await _db.SaveChangesAsync(cancellationToken);
        _db.PublicationExports.AddRange(GetExportedFiles(existingPublications, allLanguages, moment));

        _db.ChangeTracker.Clear();
        await _db.RecalculateFolders(_clock.Now, cancellationToken);
        await t.CommitAsync(cancellationToken);
    }

    private void ClearDatabase()
    {
        _logger.LogInformation("Deleting languages");
        _db.Languages.ExecuteDelete();
        _logger.LogInformation("Deleting folders");
        _db.Folders.ExecuteDelete();
        _logger.LogInformation("Deleting placements");
        _db.Publications.ExecuteDelete();
        _db.PublicationPlacement.ExecuteDelete();
        _logger.LogInformation("Deleting authors");
        _db.Authors.ExecuteDelete();
        _logger.LogInformation("Deleting publication links");
        _db.PublicationLinks.ExecuteDelete();
        _logger.LogInformation("Deleting publication files");
        _db.PublicationMp3Files.ExecuteDelete();
        _db.PublicationExports.ExecuteDelete();
    }

    private List<PublicationAuthor> GetAuthors(DateTimeOffset moment)
    {
        var allAuthors = _egwDb.Query(AuthorsQuery)
            .Select(r => new
            {
                Id = (int)r.Id,
                FirstName = (string?)r.FirstName,
                MiddleName = (string?)r.MiddleName,
                LastName = (string?)r.LastName,
                Code = (string?)r.Code,
                Biography = (string?)r.Biography,
                Lifespan = (string?)r.Lifespan,
            })
            .ToArray();
        var result = new List<PublicationAuthor>();
        foreach (var author in allAuthors)
        {
            int? startYear = null;
            int? endYear = null;
            if (!string.IsNullOrWhiteSpace(author.Lifespan))
            {
                if (author.Lifespan.Contains('-'))
                {
                    string[] a = author.Lifespan.Split('-');
                    startYear = int.Parse(a[0]);
                    endYear = int.Parse(a[0]);
                }
                else
                {
                    startYear = int.Parse(author.Lifespan);
                }
            }

            result.Add(
                new PublicationAuthor(
                        author.Id,
                        author.FirstName ?? "",
                        author.MiddleName ?? "",
                        author.LastName ?? "",
                        moment
                    ).SetCode(author.Code, moment)
                    .SetBiography(author.Biography, moment)
                    .SetLifeTime(startYear, endYear, moment)
            );
        }

        return result;
    }

    private List<ExportedFile> GetExportedFiles(HashSet<int> publications, IReadOnlyCollection<Language> languages,
        DateTimeOffset moment)
    {
        IEnumerable<dynamic> data = _egwDb.Query(FilesQuery, new { publications });
        var result = new List<ExportedFile>();
        foreach (dynamic? row in data)
        {
            var fileTemplate = new
            {
                PublicationId = (int)row.PublicationId,
                ExportType = (string)row.ExportType,
                LanguageCode = FindLanguage(row.LanguageCode, languages).EgwCode,
                Code = (string)row.Code,
                CodeEn = (string?)row.CodeEn
            };
            string fileName = GetFile(
                fileTemplate.PublicationId,
                fileTemplate.ExportType,
                fileTemplate.LanguageCode,
                fileTemplate.Code,
                fileTemplate.CodeEn);
            result.Add(new ExportedFile(
                Guid.NewGuid(),
                fileTemplate.PublicationId,
                moment
            )
            {
                Type = Enums.Parse<ExportTypeEnum>(fileTemplate.ExportType, true, EnumFormat.Description),
                Uri = new Uri(fileName, UriKind.Relative),
                Size = 0,
                IsMain = false
            });
        }

        return result;
    }

    private List<Folder> GetFolders(DateTimeOffset moment)
    {
        EgwOldFolder[] allFolders = _egwDb.Query<EgwOldFolder>(FoldersQuery).ToArray();
        var targetFolders = new List<Folder>();
        foreach (EgwOldFolder? oldFolder in allFolders)
        {
            if (oldFolder.Id == 230)
            {
                continue;
            }

            string? cssClass = oldFolder.CssClass;
            if (oldFolder.ParentId == 0)
            {
                cssClass = "root";
            }

            cssClass ??= GetParentCssClass(oldFolder.Id, allFolders);

            var folder = new Folder(
                oldFolder.Title,
                oldFolder.ParentId == 0 ? null : (int)oldFolder.ParentId,
                oldFolder.Sort,
                cssClass,
                moment
            ) { Id = (int)oldFolder.Id };
            targetFolders.Add(folder);
        }

        IEnumerable<int?> distinctParents = targetFolders.Select(r => r.ParentId).Distinct();
        foreach (int? parent in distinctParents)
        {
            var children = targetFolders.Where(r => r.ParentId == parent).OrderBy(r => r.Order).ToList();
            for (int i = 0; i < children.Count; i++)
            {
                Folder child = children[i];
                child.SetPosition(parent, i + 1, moment);
            }
        }

        return targetFolders;
    }

    private IReadOnlyCollection<Language> GetLanguages(IReadOnlyCollection<Folder> targetFolders, DateTimeOffset moment)
    {
        IEnumerable<dynamic> languages = _egwDb.Query(LanguagesQuery);

        var allLanguages = new List<Language>();
        foreach (dynamic? language in languages)
        {
            string egwCode = string.IsNullOrWhiteSpace(language.EgwCode)
                ? language.Code
                : language.EgwCode;
            allLanguages.Add(
                new Language(language.Code, egwCode, language.Bcp47Code, language.Direction == "rtl",
                    language.Title,
                    moment)
                {
                    RootFolderId = targetFolders.Any(f => f.Id == language.FolderId && f.ParentId is null)
                        ? language.FolderId
                        : null
                });
        }

        foreach (Language language in allLanguages.GroupBy(r => r.EgwCode)
                     .Where(r => r.Count() > 1)
                     .SelectMany(r => r))
        {
            language.SetEgwCode(language.Code, moment);
        }

        return allLanguages;
    }

    private IReadOnlyCollection<PublicationPlacement> GetPlacements(
        DateTimeOffset moment)
    {
        var folders = _db.Folders
            .AsNoTracking()
            .Include(r => r.Type)
            .ToList();

        int[] availableFolders = folders.Select(r => r.Id).ToArray();

        var distinctFolders = _egwDb
            .Query(DistinctFoldersQuery, new { availableFolders })
            .Select(r => new
            {
                BookId = (int)r.BookId,
                FolderId = (int)r.FolderId,
                FolderOrder = (int)r.FolderOrder,
                PubType = (string)r.PubType,
                SubType = (string?)r.SubType,
            })
            .ToArray();


        var placements = distinctFolders
            .Select(fld =>
            {
                PublicationType pubType = _pubTypeDetector.GetPublicationType(fld.PubType, fld.SubType ?? "")
                                          ?? throw new ArgumentException(
                                              $"Unknown publication type for publication {fld.BookId} ({fld.PubType}.{fld.SubType})");
                if (pubType == PublicationType.BibleCommentary && fld.FolderId == 4)
                {
                    return null;
                }

                Folder parentFolder = folders.Single(r => r.Id == fld.FolderId);
                if (!parentFolder.Type.AllowedTypes.Contains(pubType))
                {
                    _logger.LogError("for {BookId}@{FolderId}Type is {Type}, allowed types = {Types}",
                        fld.BookId,
                        fld.FolderId,
                        pubType,
                        parentFolder.Type.AllowedTypes);
                    return null;
                }

                if (_permissions.TryGetValue(fld.BookId, out PublicationPermissionEnum? permission)
                    && permission is null)
                {
                    return null;
                }

                return new PublicationPlacement(fld.BookId, fld.FolderId, fld.FolderOrder, moment)
                    .ChangePermission(
                        permission ?? PublicationPermissionEnum.Public,
                        moment);
            })
            .OfType<PublicationPlacement>()
            .ToList();

        FixPlacementOrder(placements, moment);
        return placements;
    }

    private List<Publication> GetPublications(
        HashSet<int> pubList,
        IReadOnlyCollection<Language> languages,
        DateTimeOffset moment)
    {
        IEnumerable<dynamic> egwDbPublications = _egwDb.Query(PublicationsQuery, new { publications = pubList });
        var result = new List<Publication>();
        foreach (dynamic? publication in egwDbPublications)
        {
            int? pubYear = (int?)publication.PubYear;
            int? pageCount = (int?)publication.PageCount;
            string? shopLink = (string?)publication.ShopLink;
            int? englishPubId = (int?)publication.IdEn;
            if (englishPubId == 0 || englishPubId == publication.Id)
            {
                englishPubId = null;
            }

            if (englishPubId != null && !pubList.Contains(englishPubId.Value))
            {
                _logger.LogWarning("Publication {Id} has English publication {EnglishId} which is not in the list",
                    (int)publication.Id,
                    englishPubId);
                englishPubId = null;
            }

            Uri? shopUri = null;
            if (!string.IsNullOrWhiteSpace(shopLink) && Uri.TryCreate(shopLink, UriKind.Absolute, out Uri? uriTmp))
            {
                shopUri = uriTmp;
            }

            result.Add(new Publication(publication.Id,
                _pubTypeDetector.GetPublicationType(publication.PubType ?? "", publication.SubType)
                ?? throw new ArgumentException(
                    $"Invalid publication type: {(string)publication.Type}.{(string?)publication.SubType}"
                ),
                FindLanguage(publication.LanguageCode, languages).Code,
                publication.Code,
                moment
            )
            {
                OriginalPublicationId = englishPubId,
                AuthorId = publication.AuthorId > 0 ? publication.AuthorId : null,
                PublicationYear = pubYear is null or 0 ? null : pubYear.Value,
                Isbn = (string?)publication.ISBN,
                Description = (string?)publication.Description?.Trim() ?? "",
                Publisher = ((string?)publication.Publisher)?.Trim() ?? "",
                PageCount = pageCount is null or 0 ? null : pageCount.Value,
                PurchaseLink = shopUri,
            }.ChangeTitle(publication.Title, moment));
        }

        return result;
    }

    private string GetFile(int pubNr, string type, string language, string code, string? codeEn)
    {
        type = type.ToLowerInvariant();
        string filename = language != "en" && !string.IsNullOrWhiteSpace(codeEn)
            ? $"{code}({codeEn})"
            : code;

        return type == "mp3"
            ? $"/{type}/{pubNr}/{language}_{filename}.zip"
            : $"/{type}/{language}_{filename}.{type}";
    }

    private static string GetParentCssClass(uint oldFolderId, EgwOldFolder[] allFolders)
    {
        uint parent = oldFolderId;
        while (parent != 0)
        {
            EgwOldFolder folder = allFolders.Single(r => r.Id == parent);
            if (!string.IsNullOrWhiteSpace(folder.CssClass))
            {
                return folder.CssClass;
            }

            parent = folder.ParentId;
        }

        return "root";
    }

    private Language FindLanguage(string languageCode, IReadOnlyCollection<Language> languages)
    {
        Language? s = languages.FirstOrDefault(r =>
            r.Code == languageCode || r.EgwCode == languageCode || r.Bcp47Code == languageCode);

        if (s is not null)
        {
            return s;
        }

        return languages.Single(r => r.Code == "eng");
    }

    private static void FixPlacementOrder(IReadOnlyCollection<PublicationPlacement> placements, DateTimeOffset moment)
    {
        int[] parents = placements.Select(r => r.FolderId).Distinct().ToArray();
        foreach (int parentId in parents)
        {
            PublicationPlacement[] placementList = placements
                .Where(r => r.FolderId == parentId)
                .OrderBy(r => r.Order)
                .ToArray();
            for (int i = 0; i < placementList.Length; i++)
            {
                PublicationPlacement placement = placementList[i];
                placement.ChangePlacement(placement.FolderId, i + 1, moment);
            }
        }
    }

    private record EgwOldFolder(uint Id, uint ParentId, int Sort, string Title, string? CssClass);

    private const string FoldersQuery = @"-- noinspection SqlDialectInspectionForFile
select id_folder                            as Id,
       parentnode                           as ParentId,
       sort                                 as Sort,
       caption                              as Title,
       IF(parentnode = 0, 'root', cssclass) as CssClass
from sqlb_folders
where foldertype = 'writings'";

    private const string LanguagesQuery = @"-- noinspection SqlDialectInspectionForFile
select marccode          as Code,
    code              as EgwCode,
    lang_country      as Bcp47Code,
    direction         as Direction,
    id_writingsfolder as FolderId,
    direction         as Direction,
    name as Title
from egw_language
WHERE ID not in(110)
";

    private const string DistinctFoldersQuery = @"-- noinspection SqlDialectInspectionForFile
select a.pubnr                 as BookId,
       CAST(a.data1 as SIGNED) as FolderId,
       CAST(a.data2 as SIGNED) as FolderOrder,
       o.pubtype               as PubType,
       o.subtype               as SubType
from sqlb_publicationoverviewadd a
         inner join sqlb_publicationoverview o on a.pubnr = o.pubnr
         inner join sqlb_folders f on a.data1 = f.id_folder
where a.`key` = 'folder'
  and o.actversion = 1
  and o.pubtype != 'custom'
  and f.foldertype = 'writings'
  and o.pubtype not in ('EGWManuscript', 'EGWLetter')
  and a.data1 in @availableFolders";

    private const string AuthorsQuery = @"-- noinspection SqlDialectInspectionForFile
select distinct a.id         as Id,
       a.namefirst  as FirstName,
       a.namemiddle as MiddleName,
       a.namelast   as LastName,
       a.authorcode as Code,
       a.biography  as Biography,
       a.lifespan   as LifeSpan
from sqlb_authors a
         inner join sqlb_publicationoverview sp
                    on a.id = sp.authorcode
where sp.actversion = 1;
";

    private const string PublicationsQuery = @"-- noinspection SqlDialectInspectionForFile
select o.pubnr           as Id,
       o.englpubnr                                                                        as IdEn,
       o.pubtype         as PubType,
       o.subtype         as SubType,
       o.publicationcode as Code,
       o.englpublicationcode as CodeEn,
       o.publicationname as Title,
       o.authorcode      as AuthorId,
       o.pubyear         as PubYear,
       o.numberofpages   as PageCount,
       o.publisher as Publisher,
       (select marccode from egw_language l where l.name = o.language0) as LanguageCode,
       (select data1 from sqlb_publicationoverviewadd a where a.pubnr = o.pubnr and `key` = 'shoplink') as ShopLink,
       (select data1 from sqlb_publicationoverviewadd a where a.pubnr = o.pubnr and `key` = 'pubdescription') as Description,
       (select data1 from sqlb_publicationoverviewadd a where a.pubnr = o.pubnr and `key` = 'isbn') as ISBN
from sqlb_publicationoverview o
where o.actversion = 1
  and o.pubnr in @publications;
";

    private const string FilesQuery = @"-- noinspection SqlDialectInspectionForFile
select a.pubnr                                             as PublicationId,
       REPLACE(a.`key`, 'file_', '')                       as ExportType,
       po.publicationcode                                  as Code,
       (select marccode from egw_language l where l.name = po.language0) as LanguageCode,
       IF(po.pubnr = po1.pubnr, null, po1.publicationcode) as CodeEn
from sqlb_publicationoverviewadd a
         inner join sqlb_publicationoverview po on a.pubnr = po.pubnr
         left outer join sqlb_publicationoverview po1 on po.englpubnr = po1.pubnr
where po.actversion = 1
  and po1.actversion = 1
#   and (po1.pubnr is null or po.pubnr != po1.pubnr)
  and a.`key` in ('file_epub', 'file_mp3', 'file_pdf', 'file_mobi')
  and a.pubnr in @publications;

";
}