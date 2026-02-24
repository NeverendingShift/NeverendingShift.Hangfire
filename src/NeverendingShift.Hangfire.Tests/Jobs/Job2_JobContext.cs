namespace NeverendingShift.Hangfire.Tests.Jobs
{
    public class Job2_JobContext
    {
        public void Execute()
        {
            var ctx = JobContext.Current;

            Console.WriteLine($"Job {ctx.BackgroundJob.Id} executed");
        }
    }
}
