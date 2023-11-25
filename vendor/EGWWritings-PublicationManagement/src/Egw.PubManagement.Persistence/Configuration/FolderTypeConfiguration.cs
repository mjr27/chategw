using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Internal;

using EnumsNET;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Configuration;

internal class FolderTypeConfiguration : IEntityTypeConfiguration<FolderType>
{
    public void Configure(EntityTypeBuilder<FolderType> builder)
    {
        builder.HasKey(r => r.FolderTypeId);

        builder.Property(r => r.FolderTypeId)
            .HasMaxLength(64);
        builder.Property(r => r.Title)
            .HasMaxLength(400);
        builder.Property(r => r.AllowedTypes)
            .HasConversion(
                types => string.Join(
                    ' ',
                    types
                        .Select(r => r.AsString(EnumFormat.Description))
                ),
                str => str.Split(' ', StringSplitOptions.TrimEntries)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(p => EnumsNET.Enums.Parse<PublicationType>(p, true, EnumFormat.Description))
                    .ToArray()
            )
            .Metadata
            .SetValueComparer(new EnumArrayValueComparer<PublicationType>())
            ;
    }
}