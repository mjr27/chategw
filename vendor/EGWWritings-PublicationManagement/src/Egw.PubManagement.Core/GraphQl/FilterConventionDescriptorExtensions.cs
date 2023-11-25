using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Execution.Configuration;

using Microsoft.Extensions.DependencyInjection;
namespace Egw.PubManagement.Core.GraphQl;

/// <summary>
/// Extensions for <see cref="IFilterConventionDescriptor"/>
/// </summary>
public static class FilterConventionDescriptorExtensions
{
    /// <summary>
    /// Marks input field as json array
    /// </summary>
    /// <param name="descriptor"></param>
    /// <returns></returns>
    public static IFilterFieldDescriptor IsJsonArray(this IFilterFieldDescriptor descriptor)
    {
        return descriptor
            .Type<JsonFieldContainsOperationFilterInput>();
    }

    /// <summary>
    /// Marks input field as json array
    /// </summary>
    /// <param name="descriptor"></param>
    /// <returns></returns>
    public static IRequestExecutorBuilder AddJsonArrayConventions(this IRequestExecutorBuilder descriptor)
    {
        return descriptor
            .AddConvention<IFilterConvention>(new FilterConventionExtension(x => x.AddJsonArrayContainsOperation()));
    }

    /// <summary>
    /// Adds "contains" operation to json arrays
    /// </summary>
    /// <returns></returns>
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static IFilterOperationConventionDescriptor AddJsonArrayContainsOperation(
        this IFilterConventionDescriptor conventionDescriptor) => conventionDescriptor
        .AddProviderExtension(
            new QueryableFilterProviderExtension(y => y.AddFieldHandler<JsonFieldContainsOperationHandler>()))
        .Operation(CustomOperations.Includes)
        .Name("contains");
}