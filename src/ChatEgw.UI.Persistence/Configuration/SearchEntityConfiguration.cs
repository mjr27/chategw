using ChatEgw.UI.Persistence.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatEgw.UI.Persistence.Configuration;

internal class SearchEntityConfiguration : IEntityTypeConfiguration<SearchEntity>
{
    public void Configure(EntityTypeBuilder<SearchEntity> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasOne(r => r.SearchChunk)
            .WithMany(r => r.Entities);
        builder.Property(r => r.Type)
            .HasConversion<EnumDescriptionConverter<SearchEntityTypeEnum>>();
        builder.HasIndex(r => new { r.Type, r.Content });
    }
}