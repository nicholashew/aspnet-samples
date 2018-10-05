using log4net;
using System;
using System.Collections.Generic;
using System.IO;

namespace SftpDataExport.Helper
{
    public class FileHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(FileHelper));

        public static bool CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to create directory of " + path, ex);
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified directory and, if indicated, any subdirectories and files
        //  in the directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        public static bool DeleteDirectory(string path, bool recursive)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, recursive);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to delete directory of " + path, ex);
                return false;
            }
        }

        public static bool CopyFile(string sourceFileName, string destFileName, bool deleteSource = false)
        {
            try
            {
                if (!IsFileExists(sourceFileName) || string.IsNullOrWhiteSpace(destFileName))
                {
                    return false;
                }

                File.Copy(sourceFileName, destFileName, true);

                if (deleteSource)
                    DeleteFile(sourceFileName);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Failed to copy file from {0} to {1}", sourceFileName, destFileName), ex);
                return false;
            }
        }

        public static bool DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to delete file of " + path, ex);
                return false;
            }
        }

        public static bool IsFileExists(string path)
        {
            return File.Exists(path);
        }

        public static string FormatFileSize(long length)
        {
            string size = "0 B";
            if (length >= 1073741824)
                size = String.Format("{0:##.##}", length / 1073741824) + " GB";
            else if (length >= 1048576)
                size = String.Format("{0:##.##}", length / 1048576) + " MB";
            else if (length >= 1024)
                size = String.Format("{0:##.##}", length / 1024) + " KB";
            else
                size = String.Format("{0:##.##}", length) + " B";
            return size;
        }

        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        public static IEnumerable<string> GetFileList(string fileSearchPattern, string rootFolderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            string[] tmp;
            var pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);

            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern, searchOption);
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }
        }

        public static List<FileInfo> GetFileInfo(string fileSearchPattern, string rootFolderPath)
        {
            var rootDir = new DirectoryInfo(rootFolderPath);

            var dirList = new List<DirectoryInfo>(rootDir.GetDirectories("*", SearchOption.AllDirectories))
            {
                rootDir
            };

            var fileList = new List<FileInfo>();

            foreach (DirectoryInfo dir in dirList)
            {
                fileList.AddRange(dir.GetFiles(fileSearchPattern, SearchOption.TopDirectoryOnly));
            }

            return fileList;
        }

        public static void CreateTestFile(string fileName, int size)
        {
            using (var testFile = File.Create(fileName))
            {
                var random = new Random();
                for (int i = 0; i < 1024 * size; i++)
                {
                    var buffer = new byte[1024];
                    random.NextBytes(buffer);
                    testFile.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
