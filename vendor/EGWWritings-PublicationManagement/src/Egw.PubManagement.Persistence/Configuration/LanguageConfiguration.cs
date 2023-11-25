using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(r => r.Code);
        builder.HasIndex(r => r.EgwCode).IsUnique();
        builder.HasIndex(r => r.Bcp47Code).IsUnique();
        builder.HasIndex(r => r.RootFolderId).IsUnique();

        builder.HasOne(r => r.RootFolder);

        builder.Property(r => r.Code)
            .HasMaxLength(PublicationDbContext.LanguageCodeLength);
        builder.Property(r => r.EgwCode)
            .HasMaxLength(3);
        builder.Property(r => r.Bcp47Code)
            .HasMaxLength(12);
        builder.Property(r => r.Title)
            .HasMaxLength(200);
        
    }
}