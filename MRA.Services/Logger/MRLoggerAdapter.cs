

using Microsoft.Extensions.Logging;
using MRA.DTO.Logger;

namespace MRA.Services.Logger;

public class MRLoggerAdapter<T> : ILogger<T>
{
    private readonly MRLogger _mrLogger;

    public MRLoggerAdapter(MRLogger mrLogger)
    {
        _mrLogger = mrLogger;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, string> formatter)
    {
        var message = formatter(state);
        message = LogMessage(exception, message);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        message = LogMessage(exception, message);
    }

    private string LogMessage(Exception exception, string message)
    {
        if (exception != null)
        {
            message += $" Exception: {exception.Message}";
        }

        _mrLogger.Log(message);
        return message;
    }
}