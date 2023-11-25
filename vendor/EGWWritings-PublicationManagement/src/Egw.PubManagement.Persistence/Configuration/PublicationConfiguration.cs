using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Configuration;

internal class PublicationConfiguration : IEntityTypeConfiguration<Publication>
{
    public void Configure(EntityTypeBuilder<Publication> builder)
    {
        builder.HasKey(r => r.PublicationId);
        builder.HasOne(r => r.Language).WithMany().HasForeignKey(r => r.LanguageCode);
        builder.HasMany(r => r.Paragraphs)
            .WithOne(r => r.Publication)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.OriginalPublication)
            .WithMany()
            .HasForeignKey(r => r.OriginalPublicationId)
            .OnDelete(DeleteBehavior.SetNull);


        builder.Property(r => r.LanguageCode)
            .HasMaxLength(PublicationDbContext.LanguageCodeLength);
        builder.Property(r => r.Type)
            .HasMaxLength(32)
            .HasConversion<EnumDescriptionConverter<PublicationType>>();

        builder.Property(r => r.Code)
            .HasMaxLength(20);
        builder.Property(r => r.Title)
            .HasMaxLength(400);
    }
}