using HotChocolate.Data.Filters;
using HotChocolate.Types;
namespace Egw.PubManagement.Core.GraphQl;

/// <summary>
/// Contains operation for json arrays
/// </summary>
public class JsonFieldContainsOperationFilterInput : StringOperationFilterInputType
{
    /// <inheritdoc />
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Operation(CustomOperations.Includes).Type<StringType>();
    }
}