using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Application.Services;

using HotChocolate.Types;

namespace Egw.PubManagement.Application.GraphQl.Types;

/// <inheritdoc />
public class ExportedFileType : ObjectType<ExportedFileDto>
{
    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor<ExportedFileDto> descriptor)
    {
        descriptor.Field(r => r.Uri)
            .Resolve(ctx =>
                ctx.Service<IStorageWrapper>().Exports.GetUri(ctx.Parent<ExportedFileDto>().Uri.ToString()));

        base.Configure(descriptor);
    }
}