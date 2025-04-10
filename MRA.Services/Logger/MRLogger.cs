using Microsoft.Extensions.Logging;
using MRA.Infrastructure.Configuration;
using MRA.Services.Helpers;

namespace MRA.DTO.Logger;

public class MRLogger: ConsoleHelper, ILogger, IDisposable
{
    private const string DEFAULT_PREFIX = "";
    private const string DEFAULT_DATENAMEFORMAT = "yyyyMMdd_HHmmss";
    private const string DEFAULT_DATEFORMAT = "yyyy-MM-dd HH:mm:ss";

    private string _logDirectory;
    private string _logFilePath;
    private string _logFileNameDateFormat;
    private string _logDateFormat;
    private StreamWriter _streamWriter;
    private readonly AppConfiguration _appConfiguration;

    public MRLogger(AppConfiguration appConfig)
    {
        _appConfiguration = appConfig;

        _logDirectory = _appConfiguration.MRALogger.Location;
        if (!string.IsNullOrEmpty(_logDirectory))
        {

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            _logFileNameDateFormat = _appConfiguration.MRALogger.DateNameFormat ?? DEFAULT_DATENAMEFORMAT;
            var logPrefix = _appConfiguration.MRALogger.FilePrefix ?? DEFAULT_PREFIX;

            _logFilePath = Path.Combine(_logDirectory, $"{logPrefix}_{DateTime.Now.ToString(_logFileNameDateFormat)}.log");

            _streamWriter = new StreamWriter(_logFilePath, append: true)
            {
                AutoFlush = true
            };
        }
        _logDateFormat = _appConfiguration.MRALogger.DateFormat ?? DEFAULT_DATEFORMAT;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        string message = formatter(state, exception);
        Log(message, logLevel);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // Implementación para habilitar los niveles de logging según tus necesidades
        //return logLevel >= LogLevel.Information;
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        // Implementación para el manejo de scopes (puede dejarse en blanco si no se usa)
        return null;
    }

    public void Log(string message) => Log(message, LogLevel.Information);
    public void CleanLog(string message) => Log(message, LogLevel.Information, false);
    public void LogTrace(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Trace, showTime, showPrefix);
    public void LogDebug(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Debug, showTime, showPrefix);
    public void LogInformation(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Information, showTime, showPrefix);
    public void LogWarning(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Warning, showTime, showPrefix);
    public void LogError(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Error, showTime, showPrefix);
    public void LogCritical(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Critical, showTime, showPrefix);

    public void Log(string message, LogLevel level = LogLevel.Information, bool showTime = true, bool showPrefix = true)
    {
        string prefix = level switch
        {
            LogLevel.Trace => "TRACE ",
            LogLevel.Debug => "🐛 DEBUG ",
            LogLevel.Information => "INFO ",
            LogLevel.Warning => "⚠ WARN ",
            LogLevel.Error => "❌ ERROR ",
            LogLevel.Critical => "❌❌❌ FATAL ",
            _ => ""
        };

        string logMessage = (showTime ? $"{DateTime.Now.ToString(_logDateFormat)} " : "")
                            + (showPrefix ? prefix : "") + message;

        _streamWriter?.WriteLine(logMessage);

        switch (level)
        {
            case LogLevel.Trace:
                ShowMessageTrace(logMessage);
                break;
            case LogLevel.Debug:
                ShowMessageDebug(logMessage);
                break;
            case LogLevel.Information:
                ShowMessageInfo(logMessage);
                break;
            case LogLevel.Warning:
                ShowMessageWarning(logMessage);
                break;
            case LogLevel.Error:
                ShowMessageError(logMessage);
                break;
            case LogLevel.Critical:
                ShowMessageCritical(logMessage);
                break;
            default:
                ShowMessage(logMessage);
                break;
        };
    }

    public void Dispose()
    {
        _streamWriter?.Dispose();
    }
}
