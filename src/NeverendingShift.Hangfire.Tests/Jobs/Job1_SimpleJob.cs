namespace NeverendingShift.Hangfire.Tests.Jobs
{
    public class Job1_SimpleJob
    {
        public void Execute()
        {
            Console.WriteLine("Job executed");
        }
    }
}
