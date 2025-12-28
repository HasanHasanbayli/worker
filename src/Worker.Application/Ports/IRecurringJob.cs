namespace Worker.Application.Ports;

public interface IRecurringJob
{
    string JobId { get; }
    string Cron { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}