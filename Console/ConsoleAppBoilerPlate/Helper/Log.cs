using log4net;
using log4net.Core;
using System;
using System.Reflection;

namespace ConsoleAppBoilerPlate.Helper
{
    public static class Log
    {
        //private const string timeFormat = "HH:mm:ss";
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Log));
        public static readonly Level TraceLevel = new Level(Level.Debug.Value - 1000, "TRACE");

        private static readonly object _MessageLock = new object();

        static Log()
        {
            //XmlConfigurator.Configure(); // configured in AssemblyInfo.cs so we don't have to deliver the config file
        }

        public static void Trace(string message, params string[] args)
        {
            if (args == null)
            {
                Trace(message);
            }
            else
            {
                TraceFormat(_logger, message, args);
            }
        }

        public static void Debug(string message, params string[] args)
        {
            if (args == null)
            {
                _logger.Debug(message);
            }
            else
            {
                _logger.DebugFormat(message, args);
            }
        }

        public static void Debug(string message, Exception e)
        {
            _logger.Debug(message, e);
        }

        public static void Info(string message, params string[] args)
        {
            if (args == null)
            {
                _logger.Info(message);
            }
            else
            {
                _logger.InfoFormat(message, args);
            }
        }

        public static void Warn(string message, params string[] args)
        {
            if (args == null)
            {
                _logger.Warn(message);
            }
            else
            {
                _logger.WarnFormat(message, args);
            }
        }

        public static void Warn(string message, Exception e)
        {
            _logger.Warn(message + '\n', e);
        }

        public static void Error(string message, params string[] args)
        {
            if (args == null)
            {
                _logger.Error(message);
            }
            else
            {
                _logger.ErrorFormat(message, args);
            }
        }

        public static void Error(string message, Exception e)
        {
            _logger.Error(message + '\n', e);
        }

        public static void Fatal(string message, params string[] args)
        {
            if (args == null)
            {
                _logger.Fatal(message);
            }
            else
            {
                _logger.FatalFormat(message + '\n', args);
            }
        }

        public static void Fatal(string message, Exception e)
        {
            _logger.Fatal(message + '\n', e);
        }

        private static void Trace(this ILog log, string message)
        {
            log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, TraceLevel, message, null);
        }

        public static void TraceFormat(this ILog log, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, TraceLevel, formattedMessage, null);
        }
    }
}
