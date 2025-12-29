using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Worker.Application.Ports;

namespace Worker.Infrastructure.Persistence;

public sealed class SqlCommandExecutor(IConfiguration configuration) : IDbCommandExecutor
{
    private readonly string _connectionString = configuration.GetConnectionString("HangfireConnection")!;

    public async Task ExecuteAsync(
        string query,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var command = await CreateCommand(query, parameters, cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<T?> ExecuteScalarAsync<T>(
        string query,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using SqlCommand command = await CreateCommand(query, parameters, cancellationToken);

        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result == null || result == DBNull.Value)
        {
            return default;
        }

        return (T)Convert.ChangeType(result, typeof(T));
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> ExecuteDynamicAsync(
        string query,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using SqlCommand command = await CreateCommand(query, parameters, cancellationToken);
            await using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            List<Dictionary<string, object?>> results = [];
            while (await reader.ReadAsync(cancellationToken))
            {
                Dictionary<string, object?> row = new(reader.FieldCount);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    object value = reader.GetValue(i);

                    row[name] = value == DBNull.Value ? null : value;
                }

                results.Add(row);
            }

            return results;
        }
        catch
        {
            return [];
        }
    }

    private async Task<SqlCommand> CreateCommand(
        string query,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        SqlConnection connection = new(_connectionString);
        SqlCommand command = new(query, connection);

        if (parameters is { Count: > 0 })
        {
            command.Parameters.AddRange(parameters.ToArray());
        }

        await connection.OpenAsync(cancellationToken);
        return command;
    }
}