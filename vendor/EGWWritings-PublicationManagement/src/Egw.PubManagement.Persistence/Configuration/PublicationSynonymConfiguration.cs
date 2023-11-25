using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class PublicationSynonymConfiguration : IEntityTypeConfiguration<PublicationSynonym>
{
    public void Configure(EntityTypeBuilder<PublicationSynonym> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasOne(r => r.Publication)
            .WithMany();
        builder.Property(r => r.Synonym)
            .HasMaxLength(400);
        builder.HasIndex(r => new { r.PublicationId, r.Synonym })
            .IsUnique();
    }
}