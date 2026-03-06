using Hangfire.Server;

namespace NeverendingShift.Hangfire
{
    public interface IPerformingContextAccessor
    {
        PerformingContext Current { get; set; }
    }
}
