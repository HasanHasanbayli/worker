using Worker.Application.Ports;

namespace Worker.Infrastructure.Hangfire;

public class SqlJobStateService(ISqlSessionFactory sqlSessionFactory) : IJobStateService
{
    public async Task<bool> IsActiveAsync(string jobId, CancellationToken cancellationToken)
    {
        await using var sqlSession = await sqlSessionFactory.OpenAsync(cancellationToken);

        string sqlQuery = "SELECT State FROM Hangfire.RecurringJobState WHERE JobId = @JobId";

        var state = await sqlSession.ScalarAsync<bool>(
            sql: sqlQuery,
            parameters: [new SqlParameter("@JobId", jobId)],
            cancellationToken: cancellationToken);

        return state;
    }
}