using Serilog;
using Worker.Application.Ports;

namespace Worker.Infrastructure.Services;

public class LoggerService<T> : ILoggerService<T>
{
    private readonly ILogger _logger = Log.ForContext<T>();

    public void LogInformation(string message)
    {
        _logger.Information(message);
    }

    public void LogWarning(string message)
    {
        _logger.Warning(message);
    }

    public void LogError(string message, Exception? ex = null!)
    {
        if (ex == null!)
        {
            _logger.Error(message);
        }
        else
        {
            _logger.Error(ex, message);
        }
    }

    public void LogDebug(string message)
    {
        _logger.Debug(message);
    }
}