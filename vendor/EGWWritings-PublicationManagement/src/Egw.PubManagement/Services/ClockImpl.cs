using Egw.PubManagement.Core.Services;
namespace Egw.PubManagement.Services;

internal class ClockImpl : IClock
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}