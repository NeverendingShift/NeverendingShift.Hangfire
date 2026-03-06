using Hangfire.Console;

namespace NeverendingShift.Hangfire.Tests.Jobs.Common
{
    public class Logger : ILogger
    {
        private readonly IPerformingContextAccessor _contextAccessor;

        public Logger(IPerformingContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void Log(string message)
        {
            var ctx = _contextAccessor.Current;
            if (ctx is null)
                return;

            ctx.WriteLine($"{ctx.BackgroundJob.Id} - {message}");
        }
    }
}
