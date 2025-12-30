namespace Worker.Application.Ports;

public interface ILoggerService<T>
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? ex = null);
    void LogDebug(string message);
}