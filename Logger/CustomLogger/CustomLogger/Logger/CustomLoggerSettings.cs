using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomLogger.Logger
{
    public class CustomLoggerSettings
    {
        public string Path { get; set; }
        public string FilePrefix { get; set; }
        public string EmailSubject { get; set; }
        public string ToEmail { get; set; }
        public string CcEmails { get; set; }

        public List<string> CcEmailList
        {
            get { return !string.IsNullOrEmpty(CcEmails) ? CcEmails.Split(',').ToList() : new List<string>(); }
        }
    }
}
