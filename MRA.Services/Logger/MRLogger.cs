using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.Services.Helpers;
using System;
using System.IO;

namespace MRA.DTO.Logger
{
    public class MRLogger: ConsoleHelper, ILogger, IDisposable
    {
        private const string APPSETTING_LOG_PATH = "MRALogger:Location";
        private const string APPSETTING_LOG_FILE_PREFIX = "MRALogger:FilePrefix";
        private const string APPSETTING_LOG_DATE_NAME = "MRALogger:DateNameFormat";
        private const string APPSETTING_LOG_DATE_FORMAT = "MRALogger:DateFormat";

        private string _logDirectory;
        private string _logFilePath;
        private string _logFileNameDateFormat;
        private string _logDateFormat;
        private StreamWriter _streamWriter;

        public MRLogger(IConfiguration configuration)
        {
            _logDirectory = configuration[APPSETTING_LOG_PATH];
            if (!string.IsNullOrEmpty(_logDirectory))
            {

                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }

                _logFileNameDateFormat = configuration[APPSETTING_LOG_DATE_NAME] ?? "yyyyMMdd_HHmmss";
                var logPrefix = configuration[APPSETTING_LOG_FILE_PREFIX] ?? "";

                _logFilePath = Path.Combine(_logDirectory, $"{logPrefix}_{DateTime.Now.ToString(_logFileNameDateFormat)}.log");

                _streamWriter = new StreamWriter(_logFilePath, append: true)
                {
                    AutoFlush = true
                };
            }
            _logDateFormat = configuration[APPSETTING_LOG_DATE_FORMAT] ?? "yyyy-MM-dd HH:mm:ss";
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
}
