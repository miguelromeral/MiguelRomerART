using Microsoft.Extensions.Configuration;
using MRA.Services.Helpers;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Logger
{
    public class Logger : IDisposable
    {
        private const string APPSETTING_LOG_PATH = "Logger:Location";
        private const string APPSETTING_LOG_DATE_NAME = "Logger:DateNameFormat";
        private const string APPSETTING_LOG_DATE_FORMAT = "Logger:DateFormat";
        private const string APPSETTING_LOG_FILE_PREFIX = "Logger:FilePrefix";
        
        private readonly string _logDirectory;
        private readonly string _logFilePath;
        private readonly string _logFileNameDateFormat;
        private readonly string _logDateFormat;
        private StreamWriter _streamWriter;
        private ConsoleHelper _console;

        public enum LogLevel
        {
            Default,
            Info,
            Warning,
            Error,
            Success,
        }

        public Logger(IConfiguration configuration, ConsoleHelper console)
        {
            _console = console;
            _logDirectory = configuration[APPSETTING_LOG_PATH] ?? "Logs";

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            _logFileNameDateFormat = configuration[APPSETTING_LOG_DATE_NAME] ?? "yyyyMMdd_HHmmss";
            var logPrefix = configuration[APPSETTING_LOG_FILE_PREFIX] ?? "";

            // Configura el nombre del archivo con fecha y hora al inicio de la instancia de Logger
            _logFilePath = Path.Combine(_logDirectory, $"{logPrefix}_{DateTime.Now.ToString(_logFileNameDateFormat)}.log");

            _logDateFormat = configuration[APPSETTING_LOG_DATE_FORMAT] ?? "yyyy-MM-dd HH:mm:ss";

            // Abre el archivo de log con StreamWriter en modo Append
            _streamWriter = new StreamWriter(_logFilePath, append: true)
            {
                AutoFlush = true // Para que se escriba inmediatamente en el archivo
            };
        }

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
                LogLevel.Info =>    "ℹ INFO ",
                LogLevel.Warning => "⚠ WARNING ",
                LogLevel.Error =>   "❌ ERROR ",
                LogLevel.Success => "✅ SUCCESS ",
                _ =>                "",
            };

            string logMessage = (showTime ? $"{DateTime.Now.ToString(_logDateFormat)} " : "")
                                + (showPrefix ? prefix : "") + message;

            _streamWriter.WriteLine(logMessage);

            switch(level)
            {
                case LogLevel.Info: 
                    _console.ShowMessageInfo(logMessage);
                    break;
                case LogLevel.Warning: 
                    _console.ShowMessageWarning(logMessage);
                    break;
                case LogLevel.Error:
                    _console.ShowMessageError(logMessage);
                    break;
                case LogLevel.Success:
                    _console.ShowMessageSuccess(logMessage);
                    break;
                default:
                    _console.ShowMessage(logMessage);
                    break;
            };
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
        }
    }
}
