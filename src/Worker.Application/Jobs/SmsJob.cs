using Worker.Application.JobState;
using Worker.Application.Scheduling;
using Worker.Application.Services;

namespace Worker.Application.Jobs;

public class SmsJob(IJobStateService jobStateService, ISmsService smsService) : IRecurringJob
{
    public string JobId => "sms-recurring-job";
    public string Cron => "* * * * * *";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var isActive = await jobStateService.IsActiveAsync(JobId, cancellationToken);
        if (!isActive) return;

        await smsService.SendSmsAsync();

        Console.WriteLine("Sms job recurring at: " + DateTime.Now);
    }
}