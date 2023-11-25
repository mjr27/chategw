using Microsoft.EntityFrameworkCore;

namespace ChatEgw.UI.Persistence;

public class SearchDbContext : DbContext
{
    public DbSet<SearchNode> Nodes { get; private set; } = null!;
    public DbSet<SearchParagraph> Paragraphs { get; private set; } = null!;
    public DbSet<SearchParagraphReference> References { get; private set; } = null!;
    public DbSet<SearchChunk> Chunks { get; private set; } = null!;
    public DbSet<SearchEntity> Entities { get; private set; } = null!;

    public SearchDbContext(DbContextOptions<SearchDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SearchDbContext).Assembly);
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();

        base.OnConfiguring(optionsBuilder);
    }
}