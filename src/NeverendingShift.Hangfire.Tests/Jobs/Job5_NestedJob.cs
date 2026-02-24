using Hangfire;

namespace NeverendingShift.Hangfire.Tests.Jobs
{
    public class Job5_NestedJob
    {
        private readonly Common.ILogger logger;

        public Job5_NestedJob(Common.ILogger logger)
        {
            this.logger = logger;
        }

        public void Execute()
        {
            logger.Log($"{nameof(Job5_NestedJob)} Starting nested job");

            BackgroundJob.Enqueue(() => InnerJob());
            Thread.Sleep(1000);

            logger.Log($"{nameof(Job5_NestedJob)} executed successfully");
        }

        public async Task InnerJob()
        {
            logger.Log($"{nameof( InnerJob)} before delay");
            await Task.Delay(1000);
            logger.Log($"{nameof(InnerJob)} after delay");
        }
    }
}
