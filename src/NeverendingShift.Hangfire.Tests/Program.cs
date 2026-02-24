
using Hangfire;
using Hangfire.Console;
using NeverendingShift.Hangfire.Tests.Jobs;
using NeverendingShift.Hangfire.Tests.Jobs.Common;

namespace NeverendingShift.Hangfire.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHangfire(config =>
            {
                config.UseInMemoryStorage();
                config.UseNeverendingShiftJobContext();
                config.UseConsole();
            });

            builder.Services.AddHangfireServer();

            builder.Services.AddTransient<Jobs.Common.ILogger, Logger>();

            var app = builder.Build();

            app.UseHangfireDashboard();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            RecurringJob.AddOrUpdate<Job1_SimpleJob>(nameof(Job1_SimpleJob), job => job.Execute(), Cron.Never);
            RecurringJob.AddOrUpdate<Job2_JobContext>(nameof(Job2_JobContext), job => job.Execute(), Cron.Never);
            RecurringJob.AddOrUpdate<Job3_RetentionTime>(nameof(Job3_RetentionTime), job => job.Execute(), Cron.Never);
            RecurringJob.AddOrUpdate<Job4_Logger>(nameof(Job4_Logger), job => job.Execute(), Cron.Never);
            RecurringJob.AddOrUpdate<Job5_NestedJob>(nameof(Job5_NestedJob), job => job.Execute(), Cron.Never);

            app.Run();
        }
    }
}
