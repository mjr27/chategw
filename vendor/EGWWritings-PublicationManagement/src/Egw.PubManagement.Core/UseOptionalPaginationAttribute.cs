using System.Runtime.CompilerServices;

using HotChocolate.Types;
namespace Egw.PubManagement.Core;

/// <summary>
/// Uses WhiteEstate Pagination (With total count)
/// </summary>
public class UseOptionalPaginationAttribute : UseOffsetPagingAttribute
{
    /// <inheritdoc />
    // ReSharper disable once ExplicitCallerInfoArgument
    public UseOptionalPaginationAttribute(Type? type = null, [CallerLineNumber] int order = 0) : base(type, order)
    {
        IncludeTotalCount = true;
        DefaultPageSize = 1_000_000;
        MaxPageSize = 1_000_000;
    }
}