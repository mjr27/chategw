using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class ParagraphLinkConfiguration : IEntityTypeConfiguration<ParagraphLink>
{
    public void Configure(EntityTypeBuilder<ParagraphLink> builder)
    {
        builder.HasKey(r => new { r.ParaId, r.OriginalParaId });
        builder.Property(r => r.ParaId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);
        builder.Property(r => r.OriginalParaId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);
        builder.HasIndex(r => new { r.OriginalParaId });
        builder.HasIndex(r => new { r.PublicationId, r.ElementId });
        builder.HasIndex(r => new { r.OriginalPublicationId, r.OriginalElementId });
        builder.HasIndex(r => new { r.PublicationId, r.OriginalPublicationId });
    }
} 