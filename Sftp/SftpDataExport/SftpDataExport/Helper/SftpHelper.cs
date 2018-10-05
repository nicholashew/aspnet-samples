using SftpDataExport.Config;
using SftpDataExport.Models;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SftpDataExport.Helper
{
    public class SftpHelper
    {
        public static ConnectionInfo GetConnectionInfo()
        {
            var authenticationMethod = new PasswordAuthenticationMethod(AppSettings.SftpUsername, AppSettings.SftpPassword);
            return new ConnectionInfo(AppSettings.SftpHost, AppSettings.SftpPort, AppSettings.SftpUsername, authenticationMethod);
        }

        public static bool VerifyConnection(out string error)
        {
            bool success = false;

            try
            {
                using (var sftp = new SftpClient(GetConnectionInfo()))
                {
                    sftp.Connect();
                    if (sftp.IsConnected)
                    {
                        error = "";
                        success = true;
                        sftp.Disconnect();
                    }
                    else
                    {
                        error = "Failed to connect to " + sftp.ConnectionInfo.Host;
                    }
                }
            }
            catch (Exception ex)
            {
                error = "Failed to connect to SFTP. " + ex.ToString();
            }

            return success;
        }

        public static bool CreateDirectory(string remoteDirectory)
        {
            bool success = false;

            using (var sftp = new SftpClient(GetConnectionInfo()))
            {
                sftp.Connect();
                success = InternalTryCreateRemoteDirectory(sftp, remoteDirectory);
                sftp.Disconnect();
            }

            return success;
        }

        public static DownloadedFile DownloadFile(ConnectionInfo connectionInfo, string remoteDirectory, string remoteFileName, string localDirectory)
        {
            DownloadedFile downloadedFile = null;

            try
            {
                using (var sftp = new SftpClient(connectionInfo))
                {
                    try
                    {
                        Log.Trace("Connecting to " + connectionInfo.Host + " ...");
                        sftp.Connect();
                        Log.Trace("Connected to " + connectionInfo.Host);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Could not connect to " + connectionInfo.Host + " server as " + connectionInfo.Username, ex);
                    }

                    if (sftp.IsConnected)
                    {
                        var files = sftp.ListDirectory(remoteDirectory);

                        if (!Directory.Exists(localDirectory))
                        {
                            Directory.CreateDirectory(localDirectory);
                            Log.Trace("Local download directory " + localDirectory + " Created.");
                        }

                        foreach (var file in files)
                        {
                            if (!file.IsDirectory && !file.Name.StartsWith(".") && file.Name.ToLower().Trim().Equals(remoteFileName.ToLower().Trim())) //&& file.LastWriteTime.Date == DateTime.Today
                            {
                                string downloadPathName = Path.Combine(localDirectory, file.Name);
                                if (File.Exists(downloadPathName))
                                {
                                    Log.Warn("File " + file.Name + " Exists");
                                }
                                else
                                {
                                    Log.Debug("Downloading file: " + file.Name);
                                    try
                                    {
                                        using (var fs = File.OpenWrite(downloadPathName))
                                        {
                                            sftp.DownloadFile(file.FullName, fs);
                                            downloadedFile = new DownloadedFile(file, file.Name, downloadPathName, DateTime.Now);
                                        }
                                        Log.Debug("Downloaded file: " + file.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error("Failed to download file: " + file.Name, ex);
                                    }
                                }

                                // get out of the loop
                                break;
                            }
                        }
                        sftp.Disconnect();
                    }
                    else
                    {
                        Log.Warn("Could not download files because client was not connected.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("DownloadFiles Error", ex);
            }

            return downloadedFile;
        }

        public static DownloadedFile DownloadFile(string remoteDirectory, string remoteFileName, string localDirectory)
        {
            return DownloadFile(GetConnectionInfo(), remoteDirectory, remoteFileName, localDirectory);
        }

        public static List<DownloadedFile> DownloadFiles(ConnectionInfo connectionInfo, string remoteDirectory, string localDirectory)
        {
            var downloadedFiles = new List<DownloadedFile>();

            try
            {
                using (var sftp = new SftpClient(connectionInfo))
                {
                    try
                    {
                        Log.Trace("Connecting to " + connectionInfo.Host + " ...");
                        sftp.Connect();
                        Log.Trace("Connected to " + connectionInfo.Host);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Could not connect to " + connectionInfo.Host + " server as " + connectionInfo.Username, ex);
                    }

                    if (sftp.IsConnected)
                    {
                        var files = sftp.ListDirectory(remoteDirectory);

                        if (!Directory.Exists(localDirectory))
                        {
                            Directory.CreateDirectory(localDirectory);
                            Log.Trace("Local download directory " + localDirectory + " Created.");
                        }

                        foreach (var file in files)
                        {
                            if (!file.IsDirectory && !file.Name.StartsWith(".")) //&& file.LastWriteTime.Date == DateTime.Today
                            {
                                string downloadPathName = Path.Combine(localDirectory, file.Name);
                                if (File.Exists(downloadPathName))
                                {
                                    Log.Warn("File " + file.Name + " Exists");
                                }
                                else
                                {
                                    Log.Debug("Downloading file: " + file.Name);
                                    try
                                    {
                                        using (var fs = File.OpenWrite(downloadPathName))
                                        {
                                            sftp.DownloadFile(file.FullName, fs);
                                            downloadedFiles.Add(new DownloadedFile(file, file.Name, downloadPathName, DateTime.Now));
                                        }
                                        Log.Debug("Downloaded file: " + file.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error("Failed to download file: " + file.Name, ex);
                                    }
                                }
                            }
                        }
                        sftp.Disconnect();
                    }
                    else
                    {
                        Log.Warn("Could not download files because client was not connected.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("DownloadFiles Error", ex);
            }

            return downloadedFiles;
        }

        public static List<DownloadedFile> DownloadFiles(string remoteDirectory, string localDirectory)
        {
            return DownloadFiles(GetConnectionInfo(), remoteDirectory, localDirectory);
        }

        public static bool UploadFile(string remoteFileNamePath, string uploadFileNamePath, out string errorMessage)
        {
            bool success = false;

            using (var sftp = new SftpClient(GetConnectionInfo()))
            {
                sftp.Connect();
                success = InternalUploadFile(sftp, remoteFileNamePath, uploadFileNamePath, out errorMessage);
                sftp.Disconnect();
            }

            return success;
        }

        public static GenericResults UploadFilesInDirectory(string remoteDirectory, string localDirectory, string localSearchPattern = "*")
        {
            var result = new GenericResults();

            try
            {
                // Returns the names of files (including their paths) 
                string[] filePathList = Directory.GetFiles(localDirectory, localSearchPattern, SearchOption.TopDirectoryOnly);

                if (!filePathList.Any())
                {
                    Log.Debug("UploadFilesInDirectory - No files found in the directory {0}.", localDirectory);
                    result.ErrorMessages.Add("No files to be upload.");
                    return result;
                }

                using (var sftp = new SftpClient(GetConnectionInfo()))
                {
                    sftp.Connect();

                    foreach (var filePath in filePathList)
                    {
                        string remoteFileNamePath = Path.Combine(remoteDirectory, Path.GetFileName(filePath));

                        if (InternalUploadFile(sftp, remoteFileNamePath, filePath, out string errorMessage))
                        {
                            result.SuccessCount += 1;
                        }
                        else
                        {
                            result.ErrorCount += 1;
                            result.ErrorMessages.Add(errorMessage);
                        }
                    }

                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Log.Error("UploadFilesInDirectory Error", ex);
                result.ErrorMessages.Add("System Error");
            }

            return result;
        }

        private static bool InternalTryCreateRemoteDirectory(SftpClient sftp, string remoteDirectory)
        {
            try
            {
                if (!sftp.IsConnected)
                    sftp.Connect();

                if (sftp.IsConnected)
                {
                    if (sftp.Exists(remoteDirectory))
                    {
                        Log.Debug($"Remote Directory {remoteDirectory} already exists.");
                        return true;
                    }
                    else
                    {
                        sftp.CreateDirectory(remoteDirectory);
                        Log.Debug($"Remote Directory {remoteDirectory} Created.");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create remote directory {remoteDirectory}", ex);
                return false;
            }
        }

        private static bool InternalUploadFile(SftpClient sftp, string remoteFileNamePath, string uploadFileNamePath, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                if (!sftp.IsConnected)
                    sftp.Connect();

                if (sftp.IsConnected)
                {
                    Log.Debug($"Uploading {uploadFileNamePath} to remote {remoteFileNamePath}...");
                    using (var fs = File.OpenRead(uploadFileNamePath))
                    {
                        sftp.UploadFile(fs, remoteFileNamePath);
                        Log.Debug($"Upload successfully {uploadFileNamePath} to remote {remoteFileNamePath}");
                        return true;
                    }
                }

                errorMessage = "SFTP Client was not connected.";
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to upload {uploadFileNamePath} to remote {remoteFileNamePath}", ex);
                errorMessage = "SFTP Client Failed to upload.";
                return false;
            }
        }
    }
}
