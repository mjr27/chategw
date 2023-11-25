using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egw.PubManagement.Persistence.Configuration;

/// <inheritdoc />
public class ArchivedPublicationConfiguration : IEntityTypeConfiguration<ArchivedPublication>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ArchivedPublication> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => new { r.PublicationId, r.ArchivedAt });
        builder.Property(r => r.Hash)
            .HasMaxLength(64);
        builder.HasIndex(r => r.Hash)
            .IsUnique();
        builder.HasIndex(r => new { r.DeletedAt, r.PublicationId, r.ArchivedAt });
    }
}