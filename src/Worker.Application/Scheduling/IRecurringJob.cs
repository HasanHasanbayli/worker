namespace Worker.Application.Scheduling;

public interface IRecurringJob
{
    string JobId { get; }
    string Cron { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}