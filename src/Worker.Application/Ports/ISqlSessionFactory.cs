namespace Worker.Application.Ports;

public interface ISqlSessionFactory
{
    Task<ISqlSession> OpenAsync(CancellationToken ct = default);
    Task<ISqlSession> OpenAsync(string connString, CancellationToken ct = default);
}