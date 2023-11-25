using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;
using Egw.PubManagement.Persistence.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Egw.PubManagement.Persistence.Configuration;

/// <summary>
/// Global configuration settings
/// </summary>
public class GlobalOptionConfiguration : IEntityTypeConfiguration<GlobalOption>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<GlobalOption> builder)
    {
        builder.HasKey(r => r.Key);
        builder.Property(r => r.Key)
            .HasConversion<EnumDescriptionConverter<GlobalOptionTypeEnum>>()
            .HasMaxLength(32);
    }
}