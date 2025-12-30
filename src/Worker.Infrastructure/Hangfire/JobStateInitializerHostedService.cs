using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker.Application.Ports;

namespace Worker.Infrastructure.Hangfire;

public sealed class JobStateInitializerHostedService(IServiceProvider serviceProvider)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var sqlSessionFactory = scope.ServiceProvider.GetRequiredService<ISqlSessionFactory>();
        await using var sqlSession = await sqlSessionFactory.OpenAsync(cancellationToken);

        var jobs = scope.ServiceProvider.GetServices<IRecurringJob>();

        await EnsureTableAsync(sqlSession, cancellationToken);
        await SeedJobsAsync(sqlSession, jobs, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    private static Task EnsureTableAsync(
        ISqlSession sqlHelperService,
        CancellationToken cancellationToken)
    {
        const string sql = """
                               IF NOT EXISTS (
                                   SELECT 1
                                   FROM sys.tables t
                                   JOIN sys.schemas s ON t.schema_id = s.schema_id
                                   WHERE t.name = 'RecurringJobState'
                                     AND s.name = 'Hangfire'
                               )
                               BEGIN
                                   CREATE TABLE Hangfire.RecurringJobState
                                   (
                                       Id        INT IDENTITY PRIMARY KEY,
                                       JobId     NVARCHAR(100) NOT NULL,
                                       State     BIT           NOT NULL,
                                       UpdatedAt DATETIME2     NOT NULL
                                           CONSTRAINT DF_RecurringJobState_UpdatedAt
                                               DEFAULT SYSUTCDATETIME()
                                   );
                               END
                           """;

        return sqlHelperService.ExecuteAsync(sql, cancellationToken: cancellationToken);
    }

    private static async Task SeedJobsAsync(
        ISqlSession sqlHelperService,
        IEnumerable<IRecurringJob> jobs,
        CancellationToken cancellationToken)
    {
        const string sql = """
                               IF NOT EXISTS (
                                   SELECT 1
                                   FROM Hangfire.RecurringJobState
                                   WHERE JobId = @JobId
                               )
                               BEGIN
                                   INSERT INTO Hangfire.RecurringJobState (JobId, State)
                                   VALUES (@JobId, 0);
                               END
                           """;

        foreach (var job in jobs)
        {
            await sqlHelperService.ExecuteAsync(
                sql, parameters: [new SqlParameter("@JobId", job.JobId)],
                cancellationToken: cancellationToken);
        }
    }
}