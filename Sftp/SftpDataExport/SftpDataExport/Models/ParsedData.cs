using System.Collections.Generic;

namespace SftpDataExport.Models
{
    public class ParsedData
    {
        public string Name { get; set; }

        public List<IDictionary<string, object>> Data { get; set; } = new List<IDictionary<string, object>>();
    }
}
