using Hangfire.Server;

namespace NeverendingShift.Hangfire
{
    public interface IJobContextAccessor
    {
        PerformingContext Current { get; }
    }
}
