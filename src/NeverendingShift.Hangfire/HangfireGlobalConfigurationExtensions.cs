using Hangfire;

namespace NeverendingShift.Hangfire
{
    public static class HangfireGlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseNeverendingShiftJobContext(
            this IGlobalConfiguration configuration)
        {
            GlobalJobFilters.Filters.Add(new JobContextFilter());
            return configuration;
        }
    }
}
