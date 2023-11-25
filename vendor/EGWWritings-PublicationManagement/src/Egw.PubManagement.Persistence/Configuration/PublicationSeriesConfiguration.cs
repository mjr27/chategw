using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class PublicationSeriesConfiguration : IEntityTypeConfiguration<PublicationSeries>
{
    public void Configure(EntityTypeBuilder<PublicationSeries> builder)
    {
        builder.HasKey(r => r.Code);

        builder.Property(r => r.Code)
            .HasMaxLength(200);
        builder.Property(r => r.Title)
            .HasMaxLength(500);
        builder.Property(r => r.Type)
            .HasConversion<EnumDescriptionConverter<SeriesTypeEnum>>()
            .HasMaxLength(100);
        builder.Property(r => r.Publications)
            .HasColumnType("jsonb");
    }
}