using Hangfire.Server;
using System;

namespace NeverendingShift.Hangfire
{
    public interface IJobContextScopeFactory
    {
        IDisposable BeginScope(PerformingContext context);
    }
}
