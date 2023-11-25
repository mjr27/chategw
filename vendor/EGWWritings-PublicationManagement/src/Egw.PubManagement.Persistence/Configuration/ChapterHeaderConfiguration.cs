using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class ChapterHeaderConfiguration : IEntityTypeConfiguration<PublicationChapter>
{
    public void Configure(EntityTypeBuilder<PublicationChapter> builder)
    {
        builder.HasKey(r => r.ParaId);
        builder.Property(r => r.ParaId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);

        builder.Property(r => r.ContentEndParaId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);

        builder.Property(r => r.EndParaId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);

        builder.Property(r => r.ChapterId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);

        builder.HasIndex(r => r.PublicationId);

        builder.HasOne(r => r.Paragraph)
            .WithMany()
            .HasForeignKey(r => r.ParaId);

        builder.HasOne(r => r.ContentEndParagraph)
            .WithMany()
            .HasForeignKey(r => r.ContentEndParaId);
        builder.HasOne(r => r.EndParagraph)
            .WithMany()
            .HasForeignKey(r => r.EndParaId);
    }
}