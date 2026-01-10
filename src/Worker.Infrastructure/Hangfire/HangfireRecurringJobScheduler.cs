using Hangfire;
using Worker.Application.Ports;

namespace Worker.Infrastructure.Hangfire;

public class HangfireRecurringJobScheduler(IEnumerable<IRecurringJob> jobs)
{
    public void RegisterAll()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Azerbaijan Standard Time");

        foreach (IRecurringJob job in jobs)
        {
            int index = 0;
            foreach (string cron in job.Cron)
            {
                string jobId = $"{job.JobId}:{index++}";

                RecurringJob.AddOrUpdate(
                    recurringJobId: jobId,
                    methodCall: () => job.ExecuteAsync(CancellationToken.None),
                    cronExpression: cron,
                    options: new RecurringJobOptions
                    {
                        TimeZone = timeZone
                    }
                );
            }
        }
    }
}