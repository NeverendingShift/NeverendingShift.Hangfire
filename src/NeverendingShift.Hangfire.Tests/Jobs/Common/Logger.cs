using Hangfire.Console;

namespace NeverendingShift.Hangfire.Tests.Jobs.Common
{
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            var ctx = JobContext.Current;
            if (ctx is null)
                return;

            ctx.WriteLine($"{ctx.BackgroundJob.Id} - {message}");
        }
    }
}
