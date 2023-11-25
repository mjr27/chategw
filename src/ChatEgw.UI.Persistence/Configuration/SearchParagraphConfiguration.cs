using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatEgw.UI.Persistence.Configuration;

internal class SearchParagraphConfiguration : IEntityTypeConfiguration<SearchParagraph>
{
    public void Configure(EntityTypeBuilder<SearchParagraph> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.HasIndex(r => r.Content)
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("english");
        builder.HasOne(r => r.Node)
            .WithMany(r => r.Paragraphs);
    }
}