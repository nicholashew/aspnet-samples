using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SftpDataExport.Helper
{
    public class CsvUtil
    {
        public static List<T> GetRecords<T>(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var reader = new CsvReader(sr);
                return reader.GetRecords<T>().ToList();
            }
        }

        public static async Task<List<T>> GetRecordsAsync<T>(string path)
        {
            return await Task.Run(() =>
            {
                return GetRecords<T>(path);
            });
        }

        public static void WriteDictionaryRecords(List<Dictionary<string, object>> records, string path, bool includeHeader = true)
        {
            using (var sw = new StreamWriter(path))
            {
                var writer = new CsvWriter(sw);

                bool headerInited = includeHeader ? false : true;

                foreach (var record in records)
                {
                    if (!headerInited)
                    {
                        headerInited = true;
                        foreach (var kvp in record)
                        {
                            writer.WriteField(kvp.Key);
                        }
                        //ensure write end of record after the header
                        writer.NextRecord();
                    }

                    //write record field by field
                    foreach (var kvp in record)
                    {
                        writer.WriteField(kvp.Value);
                    }

                    //ensure write end of record when using WriteField method
                    writer.NextRecord();
                }
            }
        }

        public static void WriteRecords<T>(List<T> records, string path)
        {
            using (var sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
            {
                var writer = new CsvWriter(sw);
                writer.WriteRecords(records);
            }
        }

        public static async Task WriteRecordsAsync<T>(List<T> records, string path)
        {
            await Task.Run(() =>
            {
                WriteRecords(records, path);
            });
        }
    }
}
