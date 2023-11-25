using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class ParagraphMetadataConfiguration : IEntityTypeConfiguration<ParagraphMetadata>
{
    public void Configure(EntityTypeBuilder<ParagraphMetadata> builder)
    {
        builder.HasKey(r => r.ParaId);
        builder.Property(r => r.ParaId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);
        builder.HasIndex(r => r.PublicationId);

        // TODO add indexes
        // builder.HasIndex(r => new { r.PublicationId, r.Pagination.Section, r.Pagination.Paragraph });
        // builder.HasIndex(r => new { r.PublicationId, r.BibleMetadata.Book, r.BibleMetadata.Chapter });
        builder.HasIndex(r => new { r.PublicationId, r.Date });

        builder.Property(r => r.Pagination)
            .HasColumnType("jsonb");
        builder.Property(r => r.BibleMetadata)
            .HasColumnType("jsonb");
        builder.Property(r => r.LtMsMetadata)
            .HasColumnType("jsonb");
    }
}