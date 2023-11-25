using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat.Enums;

namespace Egw.PubManagement.Persistence;

/// <summary>
/// Publications database context
/// </summary>
public class PublicationDbContext : DbContext
{
    internal const int LanguageCodeLength = 5;
    internal const int ParaIdLength = 9 + 1 + 9;

    /// <summary> Cover code length </summary>
    public const int CoverTypeLength = 32;

    /// <summary> Global configuration </summary>
    public DbSet<GlobalOption> Configuration { get; private set; } = null!;

    /// <summary> Authors </summary>
    public DbSet<PublicationAuthor> Authors { get; private set; } = null!;

    /// <summary> Languages </summary>
    public DbSet<Language> Languages { get; private set; } = null!;

    /// <summary> Languages </summary>
    public DbSet<Folder> Folders { get; private set; } = null!;

    /// <summary> Folder types </summary>
    public DbSet<FolderType> FolderTypes { get; private set; } = null!;

    /// <summary> Publications </summary>
    public DbSet<Publication> Publications { get; private set; } = null!;

    /// <summary> Publication archive </summary>
    public DbSet<ArchivedPublication> PublicationArchive { get; private set; } = null!;

    /// <summary> Publications </summary>
    public DbSet<ImportedPublicationSource> PublicationSources { get; private set; } = null!;

    /// <summary> Publication versions </summary>
    public DbSet<PublicationChapter> PublicationChapters { get; private set; } = null!;

    /// <summary> Exported files </summary>
    public DbSet<ExportedFile> PublicationExports { get; private set; } = null!;

    /// <summary> Paragraph links </summary>
    public DbSet<ParagraphLink> PublicationLinks { get; private set; } = null!;

    /// <summary> Mp3 files </summary>
    public DbSet<Mp3File> PublicationMp3Files { get; private set; } = null!;

    /// <summary> Publication placement </summary>
    public DbSet<PublicationPlacement> PublicationPlacement { get; private set; } = null!;

    /// <summary> Publication series </summary>
    public DbSet<PublicationSeries> PublicationSeries { get; private set; } = null!;

    /// <summary> Synonyms </summary>
    public DbSet<PublicationSynonym> PublicationSynonyms { get; private set; } = null!;

    /// <summary> Paragraphs </summary>
    public DbSet<Paragraph> Paragraphs { get; private set; } = null!;

    /// <summary> Paragraph metadata </summary>
    public DbSet<ParagraphMetadata> ParagraphMetadata { get; private set; } = null!;

    /// <summary> Covers </summary>
    public DbSet<Cover> Covers { get; set; } = null!;

    /// <summary> Cover types </summary>
    public DbSet<CoverType> CoverTypes { get; private set; } = null!;


    /// <inheritdoc />
    public PublicationDbContext(DbContextOptions<PublicationDbContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PublicationDbContext).Assembly);

        DateTimeOffset moment = DateTimeOffset.UnixEpoch;

        modelBuilder.Entity<CoverType>().HasData(new CoverType
        {
            Code = "web", MinWidth = 546, MinHeight = 801, Description = "Web export"
        });

        modelBuilder.Entity<FolderType>().HasData(
            new("root", "Root", moment, Array.Empty<PublicationType>()),
            new("books", "Books", moment, PublicationType.Book, PublicationType.BibleCommentary),
            new("egwwritings", "EGW Writings", moment, PublicationType.Book),
            new("biography", "Biography", moment, PublicationType.Book),
            new("devotionals", "Devotionals", moment, PublicationType.Devotional),
            new("periodicals", "Periodicals", moment,
                PublicationType.PeriodicalPageBreak,
                PublicationType.PeriodicalNoPageBreak),
            new("reference", "Reference", moment, PublicationType.Book),
            new("bible", "Bible", moment, PublicationType.Bible),
            new("bible-concordances", "Bible Concordances", moment, PublicationType.Dictionary,
                PublicationType.TopicalIndex),
            new("bible-commentaries", "Bible Commentaries", moment, PublicationType.BibleCommentary),
            new("bible-sdasi", "EGW Scripture Indices", moment, PublicationType.ScriptureIndex),
            new("bible-dictionaries", "Bible Dictionaries", moment, PublicationType.Dictionary),
            new("bible-versions", "Bible Versions", moment, PublicationType.Bible),
            new("misc-collections", "Misc Collections", moment, PublicationType.Book),
            new("pamphlets", "Pamphlets", moment, PublicationType.Book),
            new("manuscript-releases", "Manuscript releases", moment, PublicationType.Book),
            new("letters-manuscripts", "Letters & manuscripts", moment, PublicationType.Manuscript),
            new("research-documents", "EGW Research Documents", moment, PublicationType.Book),
            new("reference-works", "Reference Works", moment, PublicationType.Book),
            new("recent-authors", "Recent Authors", moment, PublicationType.Book),
            new("modern-english", "Modern English", moment, PublicationType.Book),
            new("misc-titles", "Misc Titles", moment, PublicationType.Book),
            new("dictionary", "Dictionaries", moment, PublicationType.Dictionary),
            new("topical-index", "Topical indexes", moment, PublicationType.TopicalIndex),
            new("childrens-stories", "Children stories", moment, PublicationType.Book),
            new("study-guides", "Study guides", moment, PublicationType.Book),
            new("adventist-beliefs", "Adventist beliefs", moment, PublicationType.Book),
            new("apl", "Adventist Pioneer Library", moment,
                PublicationType.Book,
                PublicationType.PeriodicalPageBreak,
                PublicationType.PeriodicalNoPageBreak));

        modelBuilder.Entity<PublicationSeries>().HasData(
            Series("bc", "EGW SDA Bible Commentary", 90, 91, 92, 93, 94, 95, 96, 97),
            Series("sdabc", "SDA Bible Commentary", 12511, 12513, 12514, 12515, 12516, 12517, 12518),
            Topic("education", "Education", 10, 23, 29, 32, 103, 1976),
            Topic("parenting", "Parenting", 2, 6, 7, 8, 86, 128),
            Topic("leadership", "Leadership", 12, 35, 84, 88, 127, 14222),
            Topic("publishing", "Publishing", 16, 24, 390),
            Topic("egw_biography", "EGW Biography", 11, 41, 105, 665, 667, 668, 669, 670, 671, 672),
            Topic("church_history", "Church History", 11, 41, 104, 127, 132, 140, 141, 145, 665),
            Topic("last_day_events", "Last Day Events", 3, 26, 28, 36, 39, 100, 132, 1445),
            Topic("christian_lifestyle", "Christian Lifestyle", 13, 14, 25, 27, 31, 77, 78, 87, 108, 137, 138, 146, 148,
                386, 11974),
            Topic("devotional_readings", "Devotional Readings", 9, 17, 33, 38, 44, 74, 79, 80, 81, 89, 102, 126, 131,
                147, 149, 151, 153, 12210, 12862),
            Topic("health_and_wellness", "Health and Wellness", 20, 75, 77, 78, 110, 125, 135, 384, 387, 388),
            Topic("history_of_redemption", "History of Redemption", 84, 88, 104, 105, 106, 127, 130, 132, 140, 141, 142,
                143, 145),
            Topic("lessons_from_the_bible", "Lessons from the Bible", 15, 18, 84, 88, 90, 91, 92, 93, 94, 95, 96, 101,
                127, 130, 12119),
            Topic("church_life_and_ministry", "Church Life and Ministry", 19, 21, 22, 83, 98, 99, 100, 121, 123, 1445),
            Topic("youth_and_modern_english", "Youth and Modern English", 7, 76, 144, 1974, 1976, 1977, 1978, 1980,
                2017, 2715, 2723, 12269),
            Topic("evangelism_and_witnessing", "Evangelism and Witnessing", 1, 11, 13, 16, 30, 34, 35, 45, 152, 389,
                489, 492, 496, 806, 12399),
            Topic("christ_s_life_and_ministry", "Christ's Life and Ministry", 15, 108, 130, 144, 150),
            Topic("relationships_and_marriage", "Relationships and Marriage", 2, 4, 19, 40, 76, 77, 78, 122, 128),
            Topic("testimonies_for_the_church", "Testimonies for the Church", 112, 113, 114, 115, 116, 117, 118, 119,
                120, 2003, 2004, 2005),
            Topic("conflict_of_the_ages_series", "Conflict of the Ages Series", 84, 88, 127, 130, 132),
            Topic("the_life_of_faith_collection", "The Life of Faith Collection", 15, 29, 108, 135, 150)
        );
        return;

        PublicationSeries Topic(string code, string title, params int[] publications)
        {
            return new PublicationSeries(code, title, SeriesTypeEnum.Topic, moment, publications);
        }

        // 14329
        PublicationSeries Series(string code, string title, params int[] publications)
        {
            return new PublicationSeries(code, title, SeriesTypeEnum.Series, moment, publications);
        }
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }
}