using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatEgw.UI.Persistence.Configuration;

internal class SearchNodeConfiguration : IEntityTypeConfiguration<SearchNode>
{
    public void Configure(EntityTypeBuilder<SearchNode> builder)
    {
        builder.HasKey(r => r.Id);
    }
}