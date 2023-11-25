using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class Mp3FileConfiguration : IEntityTypeConfiguration<Mp3File>
{
    public void Configure(EntityTypeBuilder<Mp3File> builder)
    {
        builder.HasKey(r => new { r.PublicationId, r.Filename });
        builder.HasIndex(r => new { r.ParaId, r.VoiceType })
            .IsUnique();

        builder.Property(r => r.ParaId)
            .HasConversion<ParaIdConverter>()
            .HasMaxLength(PublicationDbContext.ParaIdLength);


        builder.Property(r => r.Filename)
            .HasMaxLength(400);
    }
}