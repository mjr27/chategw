using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egw.PubManagement.Persistence.Configuration;

/// <inheritdoc />
public class ImportedPublicationSourceConfiguration : IEntityTypeConfiguration<ImportedPublicationSource>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ImportedPublicationSource> builder)
    {
        builder.HasKey(r => new { r.PublicationId, r.Source });
        builder.Property(r => r.Source).HasMaxLength(200);
        builder.Property(r => r.SourceIdentifier).HasMaxLength(200);
    }
}