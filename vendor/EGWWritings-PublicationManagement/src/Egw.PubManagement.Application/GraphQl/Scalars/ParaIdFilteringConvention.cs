using HotChocolate.Data.Filters;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.GraphQl.Scalars;

/// <inheritdoc />
public class ParaIdFilteringConvention : FilterConventionExtension
{
    /// <inheritdoc />
    protected override void Configure(IFilterConventionDescriptor descriptor)
    {
        descriptor.BindRuntimeType<ParaId, ParaIdFilterInputType>();
    }
}
