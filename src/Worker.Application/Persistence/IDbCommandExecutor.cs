using Microsoft.Data.SqlClient;

namespace Worker.Application.Persistence;

public interface IDbCommandExecutor
{
    Task ExecuteAsync(
        string query,
        string? connectionString = null,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default);

    Task<T?> ExecuteScalarAsync<T>(
        string query,
        string? connectionString = null,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Dictionary<string, object?>>> ExecuteDynamicAsync(
        string query,
        string? connectionString = null,
        IReadOnlyList<SqlParameter>? parameters = null,
        CancellationToken cancellationToken = default);
}