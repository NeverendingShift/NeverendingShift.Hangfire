using Hangfire.Server;
using System;

namespace NeverendingShift.Hangfire
{
    public sealed class JobContextFilter : IServerFilter
    {
        private IDisposable _scope;

        public void OnPerforming(PerformingContext context)
        {
            _scope = JobContextAccessor.ScopeFactory.BeginScope(context);
        }

        public void OnPerformed(PerformedContext context)
        {
            _scope?.Dispose();
        }
    }
}
