using System.Linq.Expressions;
using System.Reflection;

using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Language;
namespace Egw.PubManagement.GraphQl;

internal class QueryableStringInvariantEqualsHandler : QueryableStringOperationHandler
{
    // For creating a expression tree we need the `MethodInfo` of the `ToLower` method of string
    private static readonly MethodInfo ToLower = typeof(string)
        .GetMethods()
        .Single(
            x => x.Name == nameof(string.ToLower) && x.GetParameters().Length == 0);

    // This is used to match the handler to all `eq` fields
    protected override int Operation => DefaultFilterOperations.Equals;

    public override Expression HandleOperation(
        QueryableFilterContext context,
        IFilterOperationField field,
        IValueNode value,
        object? parsedValue)
    {
        Expression property = context.GetInstance();
        if (parsedValue is string str)
        {
            // Creates and returnes the operation
            // e.g. ~> y.Street.ToLower() == "221b baker street"
            return Expression.Equal(
                Expression.Call(property, ToLower),
                Expression.Constant(str.ToLower()));
        }

        // Something went wrong 😱
        throw new InvalidOperationException();
    }

    public QueryableStringInvariantEqualsHandler(InputParser inputParser) : base(inputParser)
    {
    }
}