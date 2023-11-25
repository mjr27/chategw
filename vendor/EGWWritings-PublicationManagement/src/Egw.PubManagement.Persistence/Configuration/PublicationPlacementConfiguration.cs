using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class PublicationPlacementConfiguration : IEntityTypeConfiguration<PublicationPlacement>
{
    public void Configure(EntityTypeBuilder<PublicationPlacement> builder)
    {
        builder.HasKey(r => r.PublicationId);
        builder.HasIndex(r => new { r.FolderId, r.Order })
            .IsUnique();

        builder.HasOne(r => r.Folder)
            .WithMany(r => r.Placements);


        builder.Property(r => r.Permission)
            .HasConversion<EnumDescriptionConverter<PublicationPermissionEnum>>()
            .HasMaxLength(64);

        builder.HasOne(r => r.Publication)
            .WithOne(r => r.Placement)
            .HasForeignKey<PublicationPlacement>(r => r.PublicationId);
    }
}