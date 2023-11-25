using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatEgw.UI.Persistence.Configuration;

internal class SearchChunkConfiguration : IEntityTypeConfiguration<SearchChunk>
{
    public void Configure(EntityTypeBuilder<SearchChunk> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasOne(r => r.Paragraph)
            .WithMany(r => r.Entities);
        // builder
        //     .HasIndex(r => r.Embedding)
        //     .HasMethod("hnsw")
        //     .HasOperators("vector_l2_ops");
    }
}