using ConsoleAppBoilerPlate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ConsoleAppBoilerPlate.Config
{
    public class AppSettings
    {
        #region Settings

        public static string UniqueAppName => GetAppSetting("UniqueAppName");

        public static AppEnvironment AppEnvironment => GetAppSetting("AppEnvironment", AppEnvironment.Local);

        public static int AppTimerIntervalMiliSeconds => GetAppSetting("AppTimerIntervalMiliSeconds", 0);

        public static bool ShowApplication => GetAppSetting("ShowApplication", false);
     
        public static string SmtpServer => GetAppSetting("SmtpServer");

        public static int SmtpPort => GetAppSetting("SmtpPort", 25);

        public static string SmtpAccount => GetAppSetting("SmtpAccount");

        public static string SmtpPassword => GetAppSetting("SmtpPassword");

        public static bool SmtpEnableSsl => GetAppSetting("SmtpEnableSsl", false);

        public static string SenderName => GetAppSetting("SenderName");

        public static string SenderEmail => GetAppSetting("SenderEmail");

        public static string SystemEmailTo => GetAppSetting("SystemEmailTo");

        public static List<string> SystemEmailCcList
        {
            get
            {
                return GetAppSetting("SystemEmailCc")
                   .Split(new Char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                   .Where(x => !string.IsNullOrWhiteSpace(x))
                   .Select(s => s.Trim())
                   .ToList();
            }
        }

        #endregion

        #region Helpers

        private static T GetAppSetting<T>(string key, T defaultValue)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string value = ConfigurationManager.AppSettings[key];
                try
                {
                    if (value != null)
                    {
                        var theType = typeof(T);
                        if (theType.IsEnum)
                            return (T)Enum.Parse(theType, value.ToString(), true);
                        return (T)Convert.ChangeType(value, theType);
                    }
                    return default(T);
                }
                catch { }
            }
            return defaultValue;
        }

        private static string GetAppSetting(string key) => GetAppSetting(key, "");

        #endregion
    }
}
