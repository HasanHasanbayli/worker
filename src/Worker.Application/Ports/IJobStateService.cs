namespace Worker.Application.Ports;

public interface IJobStateService
{
    Task<bool> IsActiveAsync(string jobId, CancellationToken cancellationToken);
}