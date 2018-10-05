using Microsoft.VisualStudio.TestTools.UnitTesting;
using SftpDataExport.Config;
using SftpDataExport.Models;

namespace SftpDataExport.Tests
{
    [TestClass]
    public class AppSettingsTests
    {
        [TestMethod]
        public void GetAppSettingsTest()
        {
            Assert.AreEqual("SftpDataExport.001", AppSettings.UniqueAppName);
            Assert.AreEqual(AppEnvironment.Local, AppSettings.AppEnvironment);
            Assert.AreEqual(3000, AppSettings.AppTimerIntervalMiliSeconds);
            Assert.IsTrue(AppSettings.ShowApplication);
            Assert.AreEqual(@"c:\temp\SftpDataExport\download", AppSettings.DownloadPath);
            Assert.AreEqual(@"c:\temp\SftpDataExport\working", AppSettings.WorkingPath);
            Assert.AreEqual(@"c:\temp\SftpDataExport\archive", AppSettings.ArchivePath);
        }
    }
}