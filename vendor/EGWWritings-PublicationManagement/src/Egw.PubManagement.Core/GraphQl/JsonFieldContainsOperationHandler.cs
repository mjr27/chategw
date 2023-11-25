using System.Linq.Expressions;
using System.Reflection;

using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Language;
using HotChocolate.Types;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Core.GraphQl;

/// <inheritdoc />
public class JsonFieldContainsOperationHandler : QueryableStringOperationHandler
{
    private static readonly MethodInfo EfJsonContainsMethod = typeof(NpgsqlJsonDbFunctionsExtensions)
        .GetMethod(nameof(NpgsqlJsonDbFunctionsExtensions.JsonExists))!;

    /// <inheritdoc />
    public JsonFieldContainsOperationHandler(InputParser inputParser) : base(inputParser)
    {
    }

    /// <inheritdoc />
    protected override int Operation => CustomOperations.Includes;

    /// <inheritdoc />
    public override Expression HandleOperation(QueryableFilterContext context, IFilterOperationField field,
        IValueNode value,
        object? parsedValue)
    {
        if (parsedValue is not string str)
        {
            throw new InvalidOperationException();
        }

        return Expression.Call(
            EfJsonContainsMethod,
            Expression.Constant(EF.Functions),
            context.GetInstance(),
            Expression.Constant(str));
    }
}