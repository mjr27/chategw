using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.ParentId, r.Order });

        builder.Property(r => r.TypeId)
            .HasMaxLength(64);

        builder.Property(r => r.MaterializedPath)
            .HasMaxLength((Folder.MaterializedPathElementLength + 1) * 10);
        
        builder.Property(r => r.Title)
            .HasMaxLength(200);

        builder.HasOne(r => r.Parent)
            .WithMany(r => r.Children)
            .OnDelete(DeleteBehavior.Cascade);
    }
}