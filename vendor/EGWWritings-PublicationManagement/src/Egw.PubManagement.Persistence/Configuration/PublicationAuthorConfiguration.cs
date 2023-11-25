using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

internal class PublicationAuthorConfiguration : IEntityTypeConfiguration<PublicationAuthor>
{
    public void Configure(EntityTypeBuilder<PublicationAuthor> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.FirstName)
            .HasMaxLength(200);
        builder.Property(r => r.MiddleName)
            .HasMaxLength(200);
        builder.Property(r => r.LastName)
            .HasMaxLength(200);
        builder.Property(r => r.Code)
            .HasMaxLength(20);
    }
}