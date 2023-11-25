using HotChocolate.Data.Filters;
using HotChocolate.Types;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.GraphQl.Scalars;

/// <summary> 
/// Para ID Filter input type
/// </summary>
public class ParaIdFilterInputType : FilterInputType<ParaId>, IComparableOperationFilterInputType
{
    /// <inheritdoc />
    protected override void Configure(IFilterInputTypeDescriptor<ParaId> descriptor)
    {
        // ComparableOperationFilterInputType<>
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(ParaIdFilterInputType));
        descriptor.Operation(DefaultFilterOperations.Equals)
            .Type<ParaIdType>()
            .MakeNullable();
        descriptor.Operation(DefaultFilterOperations.NotEquals)
            .Type<ParaIdType>()
            .MakeNullable();
        descriptor.Operation(DefaultFilterOperations.In)
            .Type<ListType<ParaIdType>>()
            .MakeNullable();
        descriptor.Operation(DefaultFilterOperations.NotIn)
            .Type<ListType<ParaIdType>>()
            .MakeNullable();
        descriptor.AllowAnd(false).AllowOr(false);
        base.Configure(descriptor);
    }
}