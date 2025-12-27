using Worker.Application.JobState;
using Worker.Application.Scheduling;
using Worker.Application.Services;

namespace Worker.Application.Jobs;

public class EmailJob(IJobStateService jobStateService, IEmailService emailService) : IRecurringJob
{
    public string JobId => "email-recurring-job";
    public string Cron => "* * * * * *";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var isActive = await jobStateService.IsActiveAsync(JobId, cancellationToken);
        if (!isActive) return;

        await emailService.SendEmailAsync();

        Console.WriteLine("Email job recurring at: " + DateTime.Now);
    }
}