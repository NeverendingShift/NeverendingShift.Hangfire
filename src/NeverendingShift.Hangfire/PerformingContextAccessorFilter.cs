using Hangfire.Common;
using Hangfire.Server;

namespace NeverendingShift.Hangfire
{
    public sealed class PerformingContextAccessorFilter : IServerFilter
    {
        private readonly IPerformingContextAccessor _accessor;

        public PerformingContextAccessorFilter(IPerformingContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            _accessor.Current = null;
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            _accessor.Current = filterContext;
        }
    }
}