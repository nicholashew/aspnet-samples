using CustomLogger.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace CustomLogger.Logger
{
    public class CustomLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        private CustomLoggerSettings _loggerSettings;
        private IMailService _mailService;

        public CustomLogger(string categoryName, Func<string, LogLevel, bool> filter, CustomLoggerSettings loggerSettings, IMailService mailService)
        {
            _categoryName = categoryName;
            _filter = filter;
            _loggerSettings = loggerSettings;
            _mailService = mailService;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // Not necessary
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $@"{DateTimeOffset.Now.ToString()} {Environment.NewLine}Level: {logLevel} {Environment.NewLine}Message: {message}";

            if (exception != null)
            {
                if (exception.Source != null)
                {
                    message += Environment.NewLine + "Error Source : " + exception.Source.ToString();
                }
                if (exception.Message != null)
                {
                    message += Environment.NewLine + "Error Message : " + exception.Message.ToString();
                }
                if (exception.InnerException != null)
                {
                    message += Environment.NewLine + "Error Exception : " + exception.InnerException.ToString();
                }
            }

            message += Environment.NewLine;
            message += "---------------------------------------------------------------------------------------------";
            message += Environment.NewLine;

            WriteToLogFile(message);

            if (logLevel == LogLevel.Critical && _mailService != null)
            {
                _mailService.SendMailAsync(_loggerSettings.ToEmail, _loggerSettings.ToEmail, _loggerSettings.CcEmailList, _loggerSettings.EmailSubject, message);
            }
        }

        private void WriteToLogFile(string message)
        {
            string filePath = Path.Combine(_loggerSettings.Path, _loggerSettings.FilePrefix + DateTimeOffset.Now.ToString("MM-dd-yyyy") + ".log");

            if (!Directory.Exists(_loggerSettings.Path))
            {
                Directory.CreateDirectory(_loggerSettings.Path);
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                FileStream fs = fileInfo.Create();
                fs.Dispose();
            }

            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.Write(message);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
