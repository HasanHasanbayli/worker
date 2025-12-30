using System.Data;
using Microsoft.Data.SqlClient;
using Worker.Application.Ports;

namespace Worker.Infrastructure.Persistence;

internal sealed class SqlSession(SqlConnection connection) : ISqlSession
{
    private SqlTransaction? _transaction;

    public async Task<int> ExecuteAsync(
        string sql,
        IReadOnlyList<SqlParameter>? parameters = null,
        bool useTransaction = true,
        CancellationToken ct = default)
    {
        await using var command = CreateCommand(sql, parameters, useTransaction);
        return await command.ExecuteNonQueryAsync(ct);
    }

    public async Task<T?> ScalarAsync<T>(
        string sql,
        IReadOnlyList<SqlParameter>? parameters = null,
        bool useTransaction = true,
        CancellationToken ct = default)
    {
        await using var command = CreateCommand(sql, parameters, useTransaction);
        var result = await command.ExecuteScalarAsync(ct);

        if (result == null || result == DBNull.Value)
        {
            return default;
        }

        return (T)Convert.ChangeType(result, typeof(T));
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> QueryAsync(
        string sql,
        IReadOnlyList<SqlParameter>? parameters = null,
        bool useTransaction = true,
        CancellationToken ct = default)
    {
        try
        {
            await using var command = CreateCommand(sql, parameters, useTransaction);
            await using var reader = await command.ExecuteReaderAsync(ct);

            List<Dictionary<string, object?>> results = [];
            while (await reader.ReadAsync(ct))
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

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = (SqlTransaction)await connection.BeginTransactionAsync(ct);
    }

    public Task CommitAsync(CancellationToken ct = default)
    {
        return _transaction!.CommitAsync(ct);
    }

    public Task RollbackAsync(CancellationToken ct = default)
    {
        return _transaction!.RollbackAsync(ct);
    }

    private SqlCommand CreateCommand(
        string sql,
        IReadOnlyList<SqlParameter>? parameters,
        bool useTransaction)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;

        if (useTransaction && _transaction != null)
        {
            command.Transaction = _transaction;
        }

        if (parameters is { Count: > 0 })
        {
            var sqlParameters = parameters
                .Select(p => new Microsoft.Data.SqlClient.SqlParameter(p.Name, p.Value))
                .ToArray();

            command.Parameters.AddRange(sqlParameters);
        }

        return command;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
    
        await connection.DisposeAsync();
    }
}