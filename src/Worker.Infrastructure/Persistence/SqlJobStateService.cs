using Microsoft.Data.SqlClient;
using Worker.Application.JobState;
using Worker.Application.Persistence;

namespace Worker.Infrastructure.Persistence;

public class SqlJobStateService(IDbCommandExecutor sqlCommandExecutor) : IJobStateService
{
    public async Task<bool> IsActiveAsync(string jobId, CancellationToken cancellationToken)
    {
        string sqlQuery = "SELECT State FROM Hangfire.RecurringJobState WHERE JobId = @JobId";

        var state = await sqlCommandExecutor.ExecuteScalarAsync<bool>(
            query: sqlQuery,
            parameters: [new SqlParameter("@JobId", jobId)],
            cancellationToken: cancellationToken);

        return state;
    }
}