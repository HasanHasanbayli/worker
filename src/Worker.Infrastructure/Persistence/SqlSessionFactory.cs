using Microsoft.Data.SqlClient;
using Worker.Application.Ports;

namespace Worker.Infrastructure.Persistence;

internal sealed class SqlSessionFactory(string connectionString) : ISqlSessionFactory
{
    public async Task<ISqlSession> OpenAsync(CancellationToken ct = default)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(ct);
        return new SqlSession(connection);
    }

    public async Task<ISqlSession> OpenAsync(string connString, CancellationToken ct = default)
    {
        var connection = new SqlConnection(connString);
        await connection.OpenAsync(ct);
        return new SqlSession(connection);
    }
}