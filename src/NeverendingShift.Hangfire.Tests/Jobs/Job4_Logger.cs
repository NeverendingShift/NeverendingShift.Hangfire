namespace NeverendingShift.Hangfire.Tests.Jobs
{
    public class Job4_Logger
    {
        private readonly Common.ILogger logger;

        public Job4_Logger(Common.ILogger logger)
        {
            this.logger = logger;
        }

        public void Execute()
        {
            logger.Log($"{nameof(Job4_Logger)} executed successfully");
        }
    }
}
