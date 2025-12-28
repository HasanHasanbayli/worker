using Microsoft.Data.SqlClient;
using Worker.Application.Ports;

namespace Worker.Infrastructure.Hangfire;

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