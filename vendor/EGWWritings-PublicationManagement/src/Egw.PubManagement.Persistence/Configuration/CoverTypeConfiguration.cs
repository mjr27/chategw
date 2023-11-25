using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class CoverTypeConfiguration : IEntityTypeConfiguration<CoverType>
{
    public void Configure(EntityTypeBuilder<CoverType> builder)
    {
        builder.HasKey(r => r.Code);
        builder.Property(r => r.Code)
            .HasMaxLength(PublicationDbContext.CoverTypeLength);
    }
}