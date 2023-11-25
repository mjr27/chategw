using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class ParagraphConfiguration : IEntityTypeConfiguration<Paragraph>
{
    public void Configure(EntityTypeBuilder<Paragraph> builder)
    {
        builder.HasKey(r => r.ParaId);

        builder.Property(r => r.ParaId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);

        builder.HasIndex(r => new { r.PublicationId, r.ParaId })
            .IsUnique();
        builder.HasIndex(r => new { r.PublicationId, r.Order })
            .IsUnique();
        builder.HasIndex(r => new { r.PublicationId, r.HeadingLevel });

        builder.HasOne(r => r.Metadata)
            .WithOne(r => r.Paragraph)
            .HasForeignKey<ParagraphMetadata>(r => r.ParaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}