using Microsoft.VisualStudio.TestTools.UnitTesting;
using SftpDataExport.Config;
using SftpDataExport.Helper;
using System.Net.Mail;

namespace SftpDataExport.Tests
{
    [TestClass]
    public class EmailHelperTests
    {
        [TestMethod]
        public void SendEmailTests()
        {
            var mailFrom = new MailAddress(AppSettings.SenderEmail, AppSettings.SenderName);
            var mailTo = new MailAddress(AppSettings.SystemEmailTo);
            var sent = EmailHelper.SendEmail(mailFrom, mailTo, "Test Email", "Test Body");
            Assert.IsTrue(sent);
        }
    }
}
