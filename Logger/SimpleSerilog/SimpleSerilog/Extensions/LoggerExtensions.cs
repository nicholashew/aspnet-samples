using Microsoft.Extensions.Logging;
using System;

namespace SimpleSerilog.Extensions
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Formats and writes a trace log message with a default event id.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogTrace(this ILogger logger, Exception exception, string message = null, params object[] args)
        {
            logger.LogTrace(default(EventId), exception, message, args);
        }

        /// <summary>
        /// Formats and writes a information log message with a default event id.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogInformation(this ILogger logger, Exception exception, string message = null, params object[] args)
        {
            logger.LogInformation(default(EventId), exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message with a default event id.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogWarning(this ILogger logger, Exception exception, string message = null, params object[] args)
        {
            logger.LogWarning(default(EventId), exception, message, args);
        }

        /// <summary>
        /// Formats and writes a error log message with a default event id.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogError(this ILogger logger, Exception exception, string message = null, params object[] args)
        {
            logger.LogError(default(EventId), exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message with a default event id.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogCritical(this ILogger logger, Exception exception, string message = null, params object[] args)
        {
            logger.LogCritical(default(EventId), exception, message, args);
        }
    }
}