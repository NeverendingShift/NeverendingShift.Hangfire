namespace NeverendingShift.Hangfire.Tests.Jobs
{
    public class Job3_RetentionTime
    {
        [RetentionTime(Minutes = 30)]
        public void Execute()
        {
            Console.WriteLine("This job should be removed in 30 minutes");
        }
    }
}
