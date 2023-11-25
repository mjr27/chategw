using Egw.PubManagement.Core.Services;
namespace Egw.PubManagement.Preview.Services;

internal class ClockImpl : IClock
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}