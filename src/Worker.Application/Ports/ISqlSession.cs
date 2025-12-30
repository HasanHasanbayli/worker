namespace Worker.Application.Ports;

public interface ISqlSession : IAsyncDisposable
{
    Task<int> ExecuteAsync(
        string sql,
        IReadOnlyList<SqlParameter>? parameters = null,
        bool useTransaction = true,
        CancellationToken cancellationToken = default);

    Task<T?> ScalarAsync<T>(
        string sql,
        IReadOnlyList<SqlParameter>? parameters = null,
        bool useTransaction = true,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Dictionary<string, object?>>> QueryAsync(
        string sql,
        IReadOnlyList<SqlParameter>? parameters = null,
        bool useTransaction = true,
        CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}

public class SqlParameter(string name, object value)
{
    public string Name { get; set; } = name;
    public object Value { get; set; } = value;
}