using Hangfire.Server;

namespace NeverendingShift.Hangfire
{
    public static class JobContext
    {
        public static PerformingContext Current => JobContextAccessor.Instance.Current;
    }
}
