using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
namespace Egw.PubManagement.GraphQl;

internal class CustomFilteringConvention : FilterConvention
{
    protected override void Configure(IFilterConventionDescriptor descriptor)
    {
        descriptor.AddDefaults();
        descriptor.Provider(
            new QueryableFilterProvider(
                x => x
                    .AddFieldHandler<QueryableStringInvariantEqualsHandler>()
                    .AddFieldHandler<QueryableStringInvariantContainsHandler>()
                    .AddDefaultFieldHandlers()
            ));
    }
}