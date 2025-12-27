using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Worker.Application.Persistence;

namespace Worker.Infrastructure.Persistence;

public sealed class SqlCommandExecutor(IConfiguration configuration) : IDbCommandExecutor
{
    private readonly string _defaultConnectionString = configuration.GetConnectionString("HangfireConnection")!;

    public async Task ExecuteAsync(
        string query,
        string? connectionString = null,
        List<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        connectionString ??= _defaultConnectionString;

        await using SqlConnection connection = new(connectionString);
        await using SqlCommand command = new(query, connection);

        if (parameters != null && parameters.Count != 0)
        {
            command.Parameters.AddRange(parameters.ToArray());
        }

        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<T?> ExecuteScalarAsync<T>(
        string query,
        string? connectionString = null,
        List<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        connectionString ??= _defaultConnectionString;

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(query, connection);

        if (parameters != null && parameters.Count != 0)
        {
            command.Parameters.AddRange(parameters.ToArray());
        }

        await connection.OpenAsync(cancellationToken);

        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result == null || result == DBNull.Value)
        {
            return default;
        }

        return (T)result;
    }
}