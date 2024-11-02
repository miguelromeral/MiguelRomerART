using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.Services.Firebase;
using MRA.Services.Helpers;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Logger
{
    public class MRLogger : IDisposable
    {
        private const string APPSETTING_LOG_PATH = "Logger:Location";
        private const string APPSETTING_LOG_DATE_NAME = "Logger:DateNameFormat";
        private const string APPSETTING_LOG_DATE_FORMAT = "Logger:DateFormat";
        private const string APPSETTING_LOG_FILE_PREFIX = "Logger:FilePrefix";
        
        private string _logDirectory;
        private string _logFilePath;
        private string _logFileNameDateFormat;
        private string _logDateFormat;
        private StreamWriter _streamWriter;
        private ConsoleHelper _console;
        private ILogger _logger;

        public enum LogLevel
        {
            Default,
            Info,
            Warning,
            Error,
            Success,
        }

        public MRLogger(IConfiguration configuration, ConsoleHelper console)
        {
            _console = console;
            Init(configuration);
        }
        public MRLogger(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            Init(configuration);
        }

        private void Init(IConfiguration configuration)
        {
            _logDirectory = configuration[APPSETTING_LOG_PATH];

            if (_logDirectory != null)
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }

                _logFileNameDateFormat = configuration[APPSETTING_LOG_DATE_NAME] ?? "yyyyMMdd_HHmmss";
                var logPrefix = configuration[APPSETTING_LOG_FILE_PREFIX] ?? "";

                // Configura el nombre del archivo con fecha y hora al inicio de la instancia de Logger
                _logFilePath = Path.Combine(_logDirectory, $"{logPrefix}_{DateTime.Now.ToString(_logFileNameDateFormat)}.log");

                // Abre el archivo de log con StreamWriter en modo Append
                _streamWriter = new StreamWriter(_logFilePath, append: true)
                {
                    AutoFlush = true // Para que se escriba inmediatamente en el archivo
                };
            }

            _logDateFormat = configuration[APPSETTING_LOG_DATE_FORMAT] ?? "yyyy-MM-dd HH:mm:ss";
        }

        public bool ProductionEnvironmentAlert(bool isInProduction)
        {
            if (isInProduction)
            {
                _console.ShowMessageWarning("Este proceso va a ser ejecutado en PRODUCCIÓN.");
                var response = _console.FillBoolValue("¿Estás seguro de que quieres que se ejecute en PRODUCCIÓN?");
                if (!response)
                {
                    Info("Ejecución abortada por 1º interacción de usuario");
                    return false;
                }
                response = _console.FillBoolValue("¿Estás realmente seguro? Esta acción se podrá deshacer.");
                if (!response)
                {
                    Info("Ejecución abortada por 2ª interacción de usuario");
                    return false;
                }
                Info("El usuario es consciente de que la ejecución será en PRODUCCIÓN");
            }
            else
            {
                Info("El proceso será ejecutado en PRE-producción");
            }
            return true;
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
                LogLevel.Warning => "⚠ WARN ",
                LogLevel.Error =>   "❌ ERROR ",
                LogLevel.Success => "✅ SUCCESS ",
                _ =>                "",
            };

            string logMessage = (showTime ? $"{DateTime.Now.ToString(_logDateFormat)} " : "")
                                + (showPrefix ? prefix : "") + message;

            _streamWriter?.WriteLine(logMessage);

            switch(level)
            {
                case LogLevel.Info: 
                    _console?.ShowMessageInfo(logMessage);
                    _logger?.LogInformation(message);
                    break;
                case LogLevel.Warning: 
                    _console?.ShowMessageWarning(logMessage);
                    _logger?.LogWarning(message);
                    break;
                case LogLevel.Error:
                    _console?.ShowMessageError(logMessage);
                    _logger?.LogError(message);
                    break;
                case LogLevel.Success:
                    _console?.ShowMessageSuccess(logMessage);
                    _logger?.LogInformation("SUCCESS "+message);
                    break;
                default:
                    _console?.ShowMessage(logMessage);
                    _logger?.LogInformation(message);
                    break;
            };
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
        }
    }
}
