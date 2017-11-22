using CustomLogger.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CustomLogger.Logger
{
    public static class CustomLoggerExtensions
    {
        public static ILoggerFactory AddCustomLogger(this ILoggerFactory factory, IOptions<CustomLoggerSettings> customLoggerSettings, IMailService mailService, Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new CustomLoggerProvider(filter, customLoggerSettings, mailService));
            return factory;
        }

        public static ILoggerFactory AddCustomLogger(this ILoggerFactory factory, IOptions<CustomLoggerSettings> customLoggerSettings, IMailService mailService, LogLevel minLevel)
        {
            return AddCustomLogger(factory,
                customLoggerSettings,
                mailService,
                (_, logLevel) => logLevel >= minLevel);
        }
    }
}
