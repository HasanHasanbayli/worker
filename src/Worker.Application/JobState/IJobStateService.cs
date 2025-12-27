namespace Worker.Application.JobState;

public interface IJobStateService
{
    Task<bool> IsActiveAsync(string jobId, CancellationToken cancellationToken);
}