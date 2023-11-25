using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Application.Models.Enums;
using Egw.PubManagement.Application.Services;

using HotChocolate.Types;
namespace Egw.PubManagement.Application.GraphQl.Types;

/// <inheritdoc />
public class CoverGraphQlType : ObjectType<CoverDto>
{
    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor<CoverDto> descriptor)
    {
        descriptor.Field(r => r.Uri)
            .Resolve(ctx => ctx.Service<IStorageWrapper>().Covers.GetUri(ctx.Parent<CoverDto>().Uri.ToString()));

        descriptor.Field("thumbnail")
            .Argument("width", o => o.Type<NonNullType<IntType>>()
                .Description("Width in pixels"))
            .Argument("height", o => o.Type<NonNullType<IntType>>()
                .Description("Height in pixels"))
            .Argument("resize", o => o.Type<EnumType<ResizeTypeEnum>>()
                .Description("Resize type. Default: Auto"))
            .Resolve(ctx =>
            {
                Uri baseUri = ctx.Parent<CoverDto>().Uri;
                int width = ctx.ArgumentValue<int>("width");
                int height = ctx.ArgumentValue<int>("height");
                ResizeTypeEnum? resizeType = ctx.ArgumentValue<ResizeTypeEnum?>("resize");
                return ctx.Service<IImageProxyWrapper>().GetUri(baseUri, width, height, resizeType ?? ResizeTypeEnum.Auto);
            });

        descriptor.Field("type")
            .Description("Cover type")
            .Resolve(async (ctx, ct) => await ctx.DataLoader<CoverTypeByIdLoader>().LoadAsync(ctx.Parent<CoverDto>().TypeId, ct));
    }
}