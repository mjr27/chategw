using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egw.PubManagement.Persistence.Configuration;

internal class ExportedFileConfiguration : IEntityTypeConfiguration<ExportedFile>
{
    public void Configure(EntityTypeBuilder<ExportedFile> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasOne(r => r.Publication)
            .WithMany()
            .HasForeignKey(r => r.PublicationId);
        builder.HasIndex(r => new { r.IsMain, r.PublicationId, r.Type });
        builder.Property(r => r.Type)
            .HasMaxLength(32)
            .HasConversion<EnumDescriptionConverter<ExportTypeEnum>>();
    }
}