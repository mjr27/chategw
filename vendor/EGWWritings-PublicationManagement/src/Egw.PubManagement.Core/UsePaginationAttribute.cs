using System.Runtime.CompilerServices;

using HotChocolate.Types;
namespace Egw.PubManagement.Core;

/// <summary>
/// Uses WhiteEstate Pagination (With total count)
/// </summary>
public class UsePaginationAttribute : UseOffsetPagingAttribute
{
    /// <inheritdoc />
    // ReSharper disable once ExplicitCallerInfoArgument
    public UsePaginationAttribute(Type? type = null, [CallerLineNumber] int order = 0) : base(type, order)
    {
        IncludeTotalCount = true;
    }
}