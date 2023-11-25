using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class CoverConfiguration : IEntityTypeConfiguration<Cover>
{
    public void Configure(EntityTypeBuilder<Cover> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.TypeId)
            .HasMaxLength(PublicationDbContext.CoverTypeLength);
        builder.Property(r => r.Format)
            .HasConversion<EnumDescriptionConverter<MediaFormatEnum>>();


        builder.HasOne(r => r.Type)
            .WithMany(r => r.Covers)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Publication)
            .WithMany(r => r.Covers);
    }
}