using ConsoleAppBoilerPlate.Common;
using ConsoleAppBoilerPlate.Config;
using ConsoleAppBoilerPlate.Helper;
using ConsoleAppBoilerPlate.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ConsoleAppBoilerPlate
{
    class Program
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            Log.Info("Running App {0} as {1}", AppSettings.UniqueAppName, WindowsIdentity.GetCurrent().Name);

            // Sets the console window visibility
            IntPtr hWndConsole = GetConsoleWindow();
            if (hWndConsole != IntPtr.Zero)
            {
                ShowWindow(hWndConsole, AppSettings.ShowApplication ? Constants.ShowWindowCommand : Constants.HideWindowCommand);
            }

            // Named Mutexes are available computer-wide. Use a unique name.
            using (var mutex = new System.Threading.Mutex(false, AppSettings.UniqueAppName))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(AppSettings.AppTimerIntervalMiliSeconds);
                if (isAnotherInstanceOpen)
                {
                    Log.Warn("Only one instance of this app is allowed.");
                    SendSystemEmail("Multiple App Instance Denied", "Only one instance of this app is allowed.", false);
                    return;
                }

                // Run Program...
                RunProgram();
            }

            // Terminates app at the end
            Environment.Exit(0);
        }

        static void RunProgram()
        {
            string jobId = DateTime.Now.ToString("yyyyMMddhhmmss");

            try
            {
                Log.Debug("Processing in progress...");

                // do your stuff here...

                string message = EmailHelper.GetEmailContent("./MailTemplates/Sample.html", new Dictionary<string, string> {
                    { "*|Date|*", DateTime.Now.ToLongDateString() }
                });
                SendSystemEmail("RunProgram Task Completed", message);
            }
            catch (Exception ex)
            {
                Log.Error("RunProgram Error", ex);
                SendSystemEmail("RunProgram Task Error", ex.ToString());
            }
        }

        private static void SendSystemEmail(string subject, string message, bool enableCc = true)
        {
            // Append environment ending of subject
            if (AppSettings.AppEnvironment != AppEnvironment.Prod)
            {
                subject += " (" + AppSettings.AppEnvironment.ToString() + ")";
            }

            EmailHelper.SendEmail(AppSettings.SenderEmail, AppSettings.SystemEmailTo, enableCc ? AppSettings.SystemEmailCcList : null, subject, message);
        }
    }
}
