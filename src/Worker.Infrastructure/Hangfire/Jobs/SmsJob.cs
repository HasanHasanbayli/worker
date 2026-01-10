using Worker.Application.Ports;
using Worker.Application.Shared.Constants;

namespace Worker.Infrastructure.Hangfire.Jobs;

public class SmsJob(IJobStateService jobStateService, ISmsService smsService) : IRecurringJob
{
    public string JobId => "sms-recurring-job";
    public string[] Cron => [CronExpression.DailyAt0200];

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var isActive = await jobStateService.IsActiveAsync(JobId, cancellationToken);
        if (!isActive) return;

        await smsService.SendSmsAsync();

        Console.WriteLine("Sms job recurring at: " + DateTime.Now);
    }
}