using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Models;

using HotChocolate.Types;
namespace Egw.PubManagement.Application.GraphQl.Types;

/// <inheritdoc />
public class FolderGraphQlType : ObjectType<FolderDto>
{
    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor<FolderDto> descriptor)
    {
        descriptor.Field("type")
            .Description("Folder type")
            .Resolve(async (ctx, ct) =>
            {
                FolderDto parent = ctx.Parent<FolderDto>();
                FolderTypeByIdLoader loader = ctx.DataLoader<FolderTypeByIdLoader>();
                return await loader.LoadAsync(parent.TypeId, ct);
            });
        descriptor.Field("parents")
            .Description("Folder type")
            .Type<NonNullType<ListType<NonNullType<FolderGraphQlType>>>>()
            .Resolve(async (ctx, ct) =>
            {
                FolderDto parent = ctx.Parent<FolderDto>();
                FolderByIdLoader loader = ctx.DataLoader<FolderByIdLoader>();
                var parents = parent.Path.Where(r => r != parent.Id).ToList();
                IReadOnlyList<FolderDto?> result = await loader.LoadAsync(parents, ct);
                return result.OfType<FolderDto>();
            });
        descriptor.Field("language")
            .Description("Language")
            .Resolve(async (ctx, ct) =>
            {
                FolderDto parent = ctx.Parent<FolderDto>();
                LanguageByFolderLoader loader = ctx.DataLoader<LanguageByFolderLoader>();
                return await loader.LoadAsync(parent.Id, ct);
            });
        descriptor.Field("childPublicationCountRecursive")
            .Resolve(async (ctx, ct) =>
            {
                FolderDto parent = ctx.Parent<FolderDto>();
                FolderRecursivePublicationCountByIdLoader loader = ctx.DataLoader<FolderRecursivePublicationCountByIdLoader>();
                return await loader.LoadAsync(parent.Id, ct);
            });

        descriptor.Field("publications")
            .Description("Child publications")
            .Type<NonNullType<ListType<NonNullType<PublicationType>>>>()
            .Resolve(async (ctx, ct) =>
            {
                FolderDto parent = ctx.Parent<FolderDto>();
                FolderChildPublicationsLoader loader = ctx.DataLoader<FolderChildPublicationsLoader>();
                PublicationDto[] result = await loader.LoadAsync(parent.Id, ct);
                return result;
            });
    }
}