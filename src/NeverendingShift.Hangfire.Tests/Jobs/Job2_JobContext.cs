namespace NeverendingShift.Hangfire.Tests.Jobs
{
    public class Job2_JobContext
    {
        private readonly IPerformingContextAccessor _contextAccessor;

        public Job2_JobContext(IPerformingContextAccessor performingContextAccessor)
        {
            _contextAccessor = performingContextAccessor;
        }

        public void Execute()
        {
            var ctx = _contextAccessor.Current;

            Console.WriteLine($"Job {ctx.BackgroundJob.Id} executed");
        }
    }
}
