using Hangfire.Server;
using System;

namespace NeverendingShift.Hangfire
{
    public sealed class JobContextFilter : IServerFilter
    {
        private const string ScopeKey = "__NeverendingShift_JobContext_Scope";

        public void OnPerforming(PerformingContext context)
        {
            var scope = JobContextAccessor.ScopeFactory.BeginScope(context);
            context.Items[ScopeKey] = scope;
        }

        public void OnPerformed(PerformedContext context)
        {
            if (context.Items.TryGetValue(ScopeKey, out var scopeObj)
                && scopeObj is IDisposable scope)
            {
                scope.Dispose();
            }
        }
    }
}
