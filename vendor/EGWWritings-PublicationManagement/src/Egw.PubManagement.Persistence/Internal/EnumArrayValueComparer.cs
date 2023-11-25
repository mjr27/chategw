using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace Egw.PubManagement.Persistence.Internal;

/// <summary>
/// Enum array comparer
/// </summary>
/// <typeparam name="T"></typeparam>
internal class EnumArrayValueComparer<T> : ValueComparer<T[]> where T : struct, Enum
{
    public EnumArrayValueComparer() : base(
        (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToArray()
    )
    {
    }
}