namespace SftpDataExport.Models
{
    public class UploadResult
    {
        public string SourceFileNamePath { get; set; }

        public string DestinationFileNamePath { get; set; }

        public bool Success { get; set; }

        public string SuccessMessage { get; set; }

        public string ErrorMessage { get; set; }
    }
}