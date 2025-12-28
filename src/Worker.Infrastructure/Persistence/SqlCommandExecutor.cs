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
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using SqlCommand command = await CreateCommand(query, connectionString, parameters, cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<T?> ExecuteScalarAsync<T>(
        string query,
        string? connectionString = null,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using SqlCommand command = await CreateCommand(query, connectionString, parameters, cancellationToken);

        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result == null || result == DBNull.Value)
        {
            return default;
        }

        return (T)Convert.ChangeType(result, typeof(T));
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> ExecuteDynamicAsync(
        string query,
        string? connectionString = null,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        List<Dictionary<string, object?>> results = [];

        try
        {
            await using SqlCommand command = await CreateCommand(query, connectionString, parameters, cancellationToken);
            await using SqlDataReader? reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>(reader.FieldCount);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object value = reader.GetValue(i);
                    row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                }

                results.Add(row);
            }

            return results;
        }
        catch
        {
            return results;
        }
    }


    private async Task<SqlCommand> CreateCommand(
        string query,
        string? connectionString,
        IReadOnlyList<SqlParameter>? parameters,
        CancellationToken cancellationToken)
    {
        connectionString ??= _defaultConnectionString;

        SqlConnection connection = new(connectionString);
        SqlCommand command = new(query, connection);

        if (parameters is { Count: > 0 })
        {
            command.Parameters.AddRange(parameters.ToArray());
        }

        await connection.OpenAsync(cancellationToken);
        return command;
    }
}