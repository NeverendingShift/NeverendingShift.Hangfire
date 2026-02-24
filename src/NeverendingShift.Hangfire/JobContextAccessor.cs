using System;

namespace NeverendingShift.Hangfire
{
    internal static class JobContextAccessor
    {
        private static IJobContextAccessor _accessor
            = new DefaultJobContextScopeFactory();

        private static IJobContextScopeFactory _scopeFactory
            = (IJobContextScopeFactory)_accessor;

        public static IJobContextAccessor Instance => _accessor;

        public static IJobContextScopeFactory ScopeFactory => _scopeFactory;

        public static void SetFactory(IJobContextScopeFactory factory)
        {
            if (factory is IJobContextAccessor accessor)
            {
                _scopeFactory = factory;
                _accessor = accessor;
            }
            else
            {
                throw new InvalidOperationException("Factory must implement IJobContextAccessor.");
            }
        }
    }
}
