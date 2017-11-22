using CustomLogger.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CustomLogger.Logger
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly CustomLoggerSettings _loggerSettings;
        private readonly IMailService _mailService;

        public CustomLoggerProvider(Func<string, LogLevel, bool> filter, IOptions<CustomLoggerSettings> loggerSettings, IMailService mailService)
        {
            _filter = filter;
            _loggerSettings = loggerSettings.Value;
            _mailService = mailService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(categoryName, _filter, _loggerSettings, _mailService);
        }

        public void Dispose()
        {
        }
    }
}
