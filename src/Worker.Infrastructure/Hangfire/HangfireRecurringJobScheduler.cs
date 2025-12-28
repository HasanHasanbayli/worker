using Hangfire;
using Worker.Application.Ports;

namespace Worker.Infrastructure.Hangfire;

public class HangfireRecurringJobScheduler(IEnumerable<IRecurringJob> jobs)
{
    public void RegisterAll()
    {
        foreach (var job in jobs)
        {
            RecurringJob.AddOrUpdate(
                job.JobId,
                () => job.ExecuteAsync(CancellationToken.None),
                job.Cron
            );
        }
    }
}