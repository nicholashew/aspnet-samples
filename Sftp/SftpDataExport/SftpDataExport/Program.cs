using SftpDataExport.Common;
using SftpDataExport.Config;
using SftpDataExport.Helper;
using SftpDataExport.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SftpDataExport
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

                // Basic validation for sftp config
                if (string.IsNullOrEmpty(AppSettings.SftpHost)
                    || string.IsNullOrEmpty(AppSettings.SftpUsername)
                    || string.IsNullOrEmpty(AppSettings.SftpPassword)
                    || string.IsNullOrEmpty(AppSettings.SftpDownloadDirectory)
                    || string.IsNullOrEmpty(AppSettings.SftpDownloadFilename)
                    || string.IsNullOrEmpty(AppSettings.SftpUploadDirectory))
                {
                    Log.Error("Missing SFTP Configuration.");
                    SendSystemEmail(Constants.StatusEmailSubject + " - Configuration Error", "Missing SFTP Configuration.");
                    return;
                }

                if (!SftpHelper.VerifyConnection(out string error))
                {
                    Log.Error("Unable to connect to SFTP, " + error);
                    SendSystemEmail(Constants.StatusEmailSubject + " - Failed to connect to SFTP", "Please ensure the SFTP Configuration is valid.<h3>Exception</h3>" + error);
                    return;
                }

                string downloadFolderPath = Path.Combine(AppSettings.DownloadPath, jobId);
                string workingFolderPath = Path.Combine(AppSettings.WorkingPath, jobId);

                var parser = new DataParser();
                parser.LoadParserSettings();

                var allData = new List<ParsedData>();
                var failedRows = new List<Tuple<int, string, string>>();

                // Download from sftp remote server                
                DownloadedFile downloadedFile = SftpHelper.DownloadFile(AppSettings.SftpDownloadDirectory, AppSettings.SftpDownloadFilename, downloadFolderPath);

                // Terminate if no files available for download
                if (downloadedFile == null)
                {
                    Log.Error("No files available for download");
                    SendSystemEmail(Constants.StatusEmailSubject, "No files available for download.");
                    FileHelper.DeleteFile(downloadFolderPath);
                    return;
                }

                //Read from downloaded csv and parse into order list
                var rows = CsvUtil.GetRecords<InOrderConfirmation>(downloadedFile.FullName);

                var rowIndex = 1; //csv header consider as 1 row
                foreach (var row in rows)
                {
                    rowIndex++;
                    var data = parser.ParseXML(row.OrderXML, out List<string> errors, enableThrowException: false);
                    if (data != null && !errors.Any())
                    {
                        allData.AddRange(data);
                    }
                    else
                    {
                        failedRows.Add(new Tuple<int, string, string>(rowIndex, row.OrderNumber, string.Join(",", errors)));
                    }
                }

                // Terminate if any invalid rows
                if (failedRows.Any())
                {
                    Log.Error("Invalid Records Found");
                    SendParseErrorEmail(jobId, failedRows);
                    ArchiveFiles(jobId, true);
                    return;
                }

                // Process Export and Upload
                var results = new Dictionary<string, UploadResult>();

                if (allData.Any())
                {
                    FileHelper.CreateDirectory(workingFolderPath);

                    // Flatten the data 
                    var flattenData = allData.GroupBy(x => x.Name)
                        .Select(group => new ParsedData
                        {
                            Name = group.Key,
                            Data = group.SelectMany(item => item.Data).Distinct().ToList()
                        }).ToList();

                    // Export csv and upload to SFTP
                    foreach (var item in flattenData)
                    {
                        string filename = item.Name + "_" + jobId + ".csv";
                        string path = workingFolderPath + " /" + filename;

                        try
                        {
                            parser.ExportAsCsv(item.Data, path);

                            var result = UploadExportCsvToSftp(AppSettings.SftpUploadDirectory, path);
                            results.Add(filename, result);
                        }
                        catch (Exception ex)
                        {
                            results.Add(filename, new UploadResult
                            {
                                Success = false,
                                ErrorMessage = "Failed to export csv for upload. Exception: " + ex.ToString()
                            });
                        }
                    }
                }

                // Send status result email
                SendStatusEmail(jobId, results);
            }
            catch (Exception ex)
            {
                Log.Error("RunProgram Error", ex);
                SendSystemEmail(Constants.StatusEmailSubject + " - Task Error", ex.ToString());
            }
            finally
            {
                ArchiveFiles(jobId, true);
            }
        }

        private static UploadResult UploadExportCsvToSftp(string remoteDirectory, string uploadFileNamePath)
        {
            var result = new UploadResult
            {
                SourceFileNamePath = uploadFileNamePath,
                DestinationFileNamePath = Path.Combine(remoteDirectory, Path.GetFileName(uploadFileNamePath))
            };

            if (SftpHelper.CreateDirectory(remoteDirectory))
            {
                if (SftpHelper.UploadFile(result.DestinationFileNamePath, uploadFileNamePath, out string errorMessage))
                {
                    result.Success = true;
                    result.SuccessMessage = "The upload has completed successfully";
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = errorMessage;
                }
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = "Unable to create upload folder in remote server";
            }

            return result;
        }

        private static void ArchiveFiles(string jobId, bool deleteSources)
        {
            string downloadFolderPath = Path.Combine(AppSettings.DownloadPath, jobId);
            string workingFolderPath = Path.Combine(AppSettings.WorkingPath, jobId);
            string archiveFolderPath = Path.Combine(AppSettings.ArchivePath, jobId);

            // Copy Source
            FileHelper.CreateDirectory(archiveFolderPath);
            FileHelper.CopyFile(downloadFolderPath + "/" + AppSettings.SftpDownloadFilename, archiveFolderPath + "/" + AppSettings.SftpDownloadFilename);

            // Copy Export
            FileHelper.CreateDirectory(archiveFolderPath + "/Export");

            if (Directory.Exists(workingFolderPath))
            {
                var files = FileHelper.GetFileList("*.csv", workingFolderPath);

                Parallel.ForEach(files, (currentFile) =>
                {
                    try
                    {
                        string filename = Path.GetFileName(currentFile);
                        FileHelper.CopyFile(currentFile, archiveFolderPath + "/Export/" + filename);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("ArchiveFiles Copy Export Error", ex);
                    }
                });
            }

            if (deleteSources)
            {
                FileHelper.DeleteDirectory(downloadFolderPath, true);
                FileHelper.DeleteDirectory(workingFolderPath, true);
            }
        }

        private static void SendParseErrorEmail(string jobId, List<Tuple<int, string, string>> failedParseRows)
        {
            string subject = Constants.StatusEmailSubject + " - Invalid Records";

            var message = new StringBuilder();
            message.AppendFormat("Date: {0} <br/>", DateTime.Now.ToString());
            message.AppendFormat("Archive Folder: {0} <br/>", Path.Combine(AppSettings.ArchivePath, jobId));

            if (failedParseRows != null)
            {
                message.Append("<h3>Invalid Records</h3>");
                message.Append("<p>Invalid records found in the downloaded csv file: </p>");
                message.Append("<table width=\"95%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">");
                message.Append("<tr><td><b>CSV Row No.</b></td><td><b>Order Number</b></td><td><b>Error Message</b></td></tr></th>");
                foreach (var item in failedParseRows)
                {
                    message.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.Item1, item.Item2, item.Item3);
                }
                message.Append("</table>");
            }

            SendSystemEmail(subject, message.ToString(), true);
        }

        private static void SendStatusEmail(string jobId, Dictionary<string, UploadResult> uploadResults)
        {
            string subject = Constants.StatusEmailSubject;

            var message = new StringBuilder();
            message.AppendFormat("Date: {0} <br/>", DateTime.Now.ToString());
            message.AppendFormat("Archive Folder: {0} <br/>", Path.Combine(AppSettings.ArchivePath, jobId));

            message.Append("<h3>Upload Result</h3>");
            if (uploadResults != null && uploadResults.Any())
            {
                message.Append("<table width=\"95%\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">");
                message.Append("<tr><td><b>File</b></td><td><b>Status</b></td><td><b>Message</b></td></tr></th>");
                foreach (var result in uploadResults)
                {
                    var statusText = result.Value.Success ? "<b style=\"color:green\">Success</b>" : "<b style=\"color:red\">Failed</b>";
                    var _message = result.Value.Success ? result.Value.SuccessMessage : result.Value.ErrorMessage;
                    message.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", result.Key, statusText, _message);
                }
                message.Append("</table>");
            }
            else
            {
                message.Append("<p>There are no records to be upload.</p>");
            }

            SendSystemEmail(subject, message.ToString(), true);
        }

        private static void SendSystemEmail(string subject, string message, bool enableCc = true, bool appendSubjectPrefix = true)
        {
            var subjectPrefix = AppSettings.SystemEmailSubjectPrefix.Trim();

            // Append subject prefix
            if (appendSubjectPrefix && !string.IsNullOrEmpty(subjectPrefix))
                subject = subjectPrefix + " - " + subject;

            // Append environment ending of subject
            if (AppSettings.AppEnvironment != AppEnvironment.Prod)
            {
                subject += " (" + AppSettings.AppEnvironment.ToString() + ")";
            }

            // Append footer message
            message += "<br/>(This is a computer generated message. Please do not reply to this message)";

            EmailHelper.SendEmail(AppSettings.SenderEmail, AppSettings.SystemEmailTo, enableCc ? AppSettings.SystemEmailCcList : null, subject, message);
        }
    }
}
