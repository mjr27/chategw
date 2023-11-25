using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Models;

using HotChocolate.Types;
namespace Egw.PubManagement.Application.GraphQl.Types;

/// <inheritdoc />
public class LanguageType : ObjectType<LanguageDto>
{
    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor<LanguageDto> descriptor)
    {
        descriptor.Field("rootFolder")
            .Resolve(async (ctx, ct) =>
            {
                LanguageDto parent = ctx.Parent<LanguageDto>();
                FolderByIdLoader loader = ctx.DataLoader<FolderByIdLoader>();
                if (parent.RootFolderId is null)
                {
                    return null;
                }

                return await loader.LoadAsync(parent.RootFolderId.Value, ct);
            });
    }
}