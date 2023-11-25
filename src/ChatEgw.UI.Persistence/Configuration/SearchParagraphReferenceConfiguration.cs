using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatEgw.UI.Persistence.Configuration;

internal class SearchParagraphReferenceConfiguration : IEntityTypeConfiguration<SearchParagraphReference>
{
    public void Configure(EntityTypeBuilder<SearchParagraphReference> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();
        builder.HasIndex(r => r.ReferenceCode);
        builder.HasOne(r => r.Paragraph)
            .WithMany(r => r.References);
    }
}