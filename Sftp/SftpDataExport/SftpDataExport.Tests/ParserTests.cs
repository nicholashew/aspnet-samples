using Microsoft.VisualStudio.TestTools.UnitTesting;
using SftpDataExport.Helper;
using SftpDataExport.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SftpDataExport.Tests
{
    [TestClass]
    public class ParserTests
    {
        internal List<InOrderConfirmation> GetSampleOrders()
        {
            return CsvUtil.GetRecords<InOrderConfirmation>("Sample/sample.csv");
        }

        [TestMethod]
        public void ReadCsvTests()
        {
            Exception exception = null;
            try
            {
                var rows = GetSampleOrders();

                Assert.IsTrue(rows != null);

                if (rows != null)
                {
                    Assert.IsTrue(rows.Any());
                    if (rows.Any())
                    {
                        Assert.AreNotEqual(string.Empty, rows[0].SubscriberKey);
                        Assert.AreNotEqual(string.Empty, rows[0].OrderNumber);
                        Assert.AreNotEqual(string.Empty, rows[0].OrderXML);
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNull(exception);
        }

        [TestMethod]
        public void ParseSettingsTests()
        {
            var parser = new DataParser();
            parser.LoadParserSettings("Sample/parser_settings.json");

            Assert.IsNotNull(parser.Settings);
            Assert.AreEqual("Order", parser.Settings?.Mapping[0]?.Name);            
            Assert.IsTrue(parser.Settings?.Mapping[0]?.Map?.Count > 0);
            Assert.AreEqual("Subscriber", parser.Settings?.Mapping[1]?.Name);
            Assert.IsTrue(parser.Settings?.Mapping[1]?.Map?.Count > 0);
            Assert.AreEqual("Products", parser.Settings?.Mapping[2]?.Name);
            Assert.IsTrue(Convert.ToBoolean(parser.Settings?.Mapping[2]?.IsLineItems));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(parser.Settings?.Mapping[2]?.LineItemPath));
            Assert.IsTrue(parser.Settings?.Mapping[2]?.Map?.Count > 0);
        }

        [TestMethod]
        public void ParseXmlTests()
        {
            try
            {
                var parser = new DataParser();
                parser.LoadParserSettings("Sample/parser_settings.json");

                var rows = GetSampleOrders();
                var failedRecords = new Dictionary<string, string>();

                var allData = new List<ParsedData>();

                foreach (var row in rows)
                {
                    var data = parser.ParseXML(row.OrderXML, out List<string> errors, false);
                    if (data != null)
                    {
                        allData.AddRange(data);
                    }
                    else
                    {
                        failedRecords.Add(row.OrderNumber, string.Join(",", errors));
                    }
                }

                Assert.IsTrue(rows.Count > 0);
                Assert.AreEqual(0, failedRecords.Count);

                // Create the test folder                
                FileHelper.CreateDirectory("temp");

                // Flatten the data 
                var flattenData = allData.GroupBy(x => x.Name)
                    .Select(group => new ParsedData
                    {
                        Name = group.Key,
                        Data = group.SelectMany(item => item.Data).Distinct().ToList()
                    }).ToList();

                foreach (var item in flattenData)
                {
                    string path = "temp/ParseXmlTests_" + item.Name + ".csv";

                    // Delete previous test file, if any
                    FileHelper.DeleteFile(path);

                    parser.ExportAsCsv(item.Data, path);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}