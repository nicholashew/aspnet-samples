using Renci.SshNet.Sftp;
using System;

namespace SftpDataExport.Models
{
    public class DownloadedFile
    {
        /// <summary>
        /// Gets the SFTP file.
        /// </summary>
        public SftpFile SftpFile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadedFile"/> class.
        /// </summary>
        /// <param name="sftpFile">The SFTP File</param>
        /// <param name="name">File name of the downloaded file </param>
        /// <param name="fullName">Full path of the downloaded file</param>
        /// <param name="downloadedTime">The time of the current file was downloaded.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sftpFile"/> is <c>null</c>.</exception>
        internal DownloadedFile(SftpFile sftpFile, string name, string fullName, DateTime downloadedTime)
        {
            SftpFile = sftpFile ?? throw new ArgumentNullException("sftpFile");
            Name = name;
            FullName = fullName;
            DownloadedTime = downloadedTime;
        }

        /// <summary>
        /// Gets the file name of the downloaded file
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the full path of the downloaded file
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets or sets the time the current file was downloaded.
        /// </summary>
        public DateTime DownloadedTime { get; private set; }

        /// <summary>
        /// Gets the name of the sftp file.
        /// </summary>
        public string RemoteFileName => SftpFile.Name;

        /// <summary>
        /// Gets the full path of the sftp file.
        /// </summary>
        public string RemoteFullName => SftpFile.FullName;

        /// <summary>
        /// Gets the time when the sftp file was last written to.
        /// </summary>
        public DateTime LastWriteTime => SftpFile.LastWriteTime;
    }
}
