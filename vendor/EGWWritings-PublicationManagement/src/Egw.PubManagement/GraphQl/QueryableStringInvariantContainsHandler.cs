using System.Linq.Expressions;
using System.Reflection;

using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Language;
namespace Egw.PubManagement.GraphQl;

internal class QueryableStringInvariantContainsHandler : QueryableStringOperationHandler
{

    public QueryableStringInvariantContainsHandler(InputParser inputParser) : base(inputParser)
    {
    }

    // For creating a expression tree we need the `MethodInfo` of the `ToLower` method of string
    private static readonly MethodInfo ToLower = typeof(string)
        .GetMethods()
        .Single(
            x => x.Name == nameof(string.ToLower) && x.GetParameters().Length == 0);
    private static readonly MethodInfo Contains = typeof(string)
        .GetMethods()
        .Single(
            x => x.Name == nameof(string.Contains)
                 && x.GetParameters().Length == 1
                 && x.GetParameters()[0].ParameterType == typeof(string) );

    // This is used to match the handler to all `eq` fields
    protected override int Operation => DefaultFilterOperations.Contains;

    public override Expression HandleOperation(
        QueryableFilterContext context,
        IFilterOperationField field,
        IValueNode value,
        object? parsedValue)
    {
        Expression property = context.GetInstance();

        if (parsedValue is string str)
        {
            return Expression.Call(
                Expression.Call(property, ToLower),
                Contains,
                Expression.Constant(str.ToLower()));
        }

        // Something went wrong 😱
        throw new InvalidOperationException();
    }

}