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

        public enum LogLevel
        {
            Default,
            Info,
            Warning,
            Error,
            Success,
        }

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

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            string message = formatter(state, exception);
            LogLevel customLogLevel = ConvertToCustomLogLevel(logLevel);
            Log(message, customLogLevel);
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            // Implementación para habilitar los niveles de logging según tus necesidades
            return logLevel >= Microsoft.Extensions.Logging.LogLevel.Information;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // Implementación para el manejo de scopes (puede dejarse en blanco si no se usa)
            return null;
        }

        private LogLevel ConvertToCustomLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel) =>
            logLevel switch
            {
                Microsoft.Extensions.Logging.LogLevel.Information => LogLevel.Info,
                Microsoft.Extensions.Logging.LogLevel.Warning => LogLevel.Warning,
                Microsoft.Extensions.Logging.LogLevel.Error => LogLevel.Error,
                _ => LogLevel.Default,
            };

        public void Log(string message) => Log(message, LogLevel.Default);
        public void CleanLog(string message) => Log(message, LogLevel.Default, false);
        public void Info(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Info, showTime, showPrefix);
        public void Warning(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Warning, showTime, showPrefix);
        public void Error(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Error, showTime, showPrefix);
        public void Success(string message, bool showTime = true, bool showPrefix = true) => Log(message, LogLevel.Success, showTime, showPrefix);

        public void Log(string message, LogLevel level = LogLevel.Default, bool showTime = true, bool showPrefix = true)
        {
            string prefix = level switch
            {
                LogLevel.Info => "ℹ INFO ",
                LogLevel.Warning => "⚠ WARN ",
                LogLevel.Error => "❌ ERROR ",
                LogLevel.Success => "✅ SUCCESS ",
                _ => ""
            };

            string logMessage = (showTime ? $"{DateTime.Now.ToString(_logDateFormat)} " : "")
                                + (showPrefix ? prefix : "") + message;

            _streamWriter?.WriteLine(logMessage);

            switch (level)
            {
                case LogLevel.Info:
                    ShowMessageInfo(logMessage);
                    break;
                case LogLevel.Warning:
                    ShowMessageWarning(logMessage);
                    break;
                case LogLevel.Error:
                    ShowMessageError(logMessage);
                    break;
                case LogLevel.Success:
                    ShowMessageSuccess(logMessage);
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
