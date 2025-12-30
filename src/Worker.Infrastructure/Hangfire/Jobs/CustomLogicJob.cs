using Worker.Application.Ports;
using Worker.Application.Shared.Constants;
using Worker.Application.UseCases;

namespace Worker.Infrastructure.Hangfire.Jobs;

public class CustomLogicJob(IJobStateService jobStateService, CalculateRetryPolicyService service) : IRecurringJob
{
    public string JobId => "custom-logic-recurring-job";
    public string Cron => CronExpression.DailyAt0200;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var isActive = await jobStateService.IsActiveAsync(JobId, cancellationToken);
        if (!isActive) return;

        service.Foo();

        Console.WriteLine("Custom logic job recurring at: " + DateTime.Now);
    }
}