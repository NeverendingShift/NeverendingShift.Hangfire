using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeverendingShift.Hangfire;

public class PerformingContextAccessor : IPerformingContextAccessor
{
    private static readonly AsyncLocal<PerformingContextHolder> _performingContextCurrent = new AsyncLocal<PerformingContextHolder>();

    public PerformingContext Current 
    {
        get => _performingContextCurrent.Value?.PerformingContext;
        set
        {
            var holder = _performingContextCurrent.Value;
            if (holder != null)
            {
                holder.PerformingContext = null;
            }
            if (value != null)
            {
                _performingContextCurrent.Value = new PerformingContextHolder(value);
            }
        }
    }

    private class PerformingContextHolder
    {
        public PerformingContextHolder(PerformingContext performingContext)
        {
            PerformingContext = performingContext;
        }

        public PerformingContext PerformingContext { get; set; }
    }
}
