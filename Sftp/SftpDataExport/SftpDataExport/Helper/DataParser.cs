using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SftpDataExport.Config;
using SftpDataExport.Extensions;
using SftpDataExport.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SftpDataExport.Helper
{
    public class DataParser
    {
        public ParserSettings Settings { get; private set; }

        public DataParser()
        {
            Settings = new ParserSettings();
        }

        public DataParser(ParserSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Load parser settings from json file, this will overrided 
        /// the current settings initialized from the constructor. 
        /// </summary>
        /// <param name="filename"></param>
        public void LoadParserSettings(string filename = "parser_settings.json")
        {
            var configFile = Path.Combine(Directory.GetCurrentDirectory(), filename);

            if (File.Exists(configFile))
            {
                //if the file exists, load the Settings
                var input = File.ReadAllText(configFile);

                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                jsonSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                jsonSettings.DefaultValueHandling = DefaultValueHandling.Populate;

                Settings = JsonConvert.DeserializeObject<ParserSettings>(input, jsonSettings);
            }
            else
            {
                Settings = new ParserSettings();
                Log.Error($"Parser settings file \"{configFile}\" not found!");
            }
        }

        /// <summary>
        /// Parsing xml string. Return null if has error.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="errors"></param>
        /// <param name="enableThrowException"></param>
        /// <returns>The Parsed Order. If any errors then return null</returns>
        public List<ParsedData> ParseXML(string xmlString, out List<string> errors, bool enableThrowException)
        {
            errors = new List<string>();
            List<ParsedData> result = null;

            try
            {
                result = new List<ParsedData>();

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                XmlNode root = xmlDoc.DocumentElement;

                foreach (var mapping in Settings.Mapping)
                {
                    var parsedData = new ParsedData
                    {
                        Name = mapping.Name
                    };

                    if (mapping.IsLineItems)
                    {
                        var lineItemNodes = root.SelectNodes(mapping.LineItemPath);
                        if (lineItemNodes != null)
                        {
                            var i = 0;
                            foreach (XmlNode lineItemNode in lineItemNodes)
                            {
                                i++;
                                var kvp = new Dictionary<string, object>();
                                foreach (var map in mapping.Map)
                                {
                                    var node = map.IsRootXPath ? root.SelectSingleNode(map.XPath) : lineItemNode?.SelectNodes(map.XPath)?[0];
                                    if (node != null)
                                    {
                                        kvp.Add(map.Name, GetNodeValue(node, map.Output, map.AttributeName));
                                    }
                                    else if (map.IsMandatory)
                                    {
                                        errors.Add("Missing XML node: " + map.XPath);
                                    }
                                }
                                parsedData.Data.Add(kvp);
                            }
                        }
                    }
                    else
                    {
                        var kvp = new Dictionary<string, object>();
                        foreach (var map in mapping.Map)
                        {
                            var node = root.SelectSingleNode(map.XPath);
                            if (node != null)
                            {
                                kvp.Add(map.Name, GetNodeValue(node, map.Output, map.AttributeName));
                            }
                            else if (map.IsMandatory)
                            {
                                errors.Add("Missing XML node: " + map.XPath);
                            }
                        }
                        parsedData.Data.Add(kvp);
                    }

                    result.Add(parsedData);
                }
            }
            catch (Exception ex)
            {
                errors.Add("Failed to parse XML");
                Log.Error("Failed to parse XML", ex);

                if (enableThrowException)
                    throw new Exception($"Failed to parse XML string: {xmlString}", ex);
            }

            return errors.Any() ? null : result;
        }

        private object GetNodeValue(XmlNode node, ParserMapOutput Output, string attributeName = "")
        {
            switch (Output)
            {
                case ParserMapOutput.Attribute:
                    return node?.Attributes[attributeName]?.Value;
                case ParserMapOutput.InnerText:
                    return node?.InnerText;
                case ParserMapOutput.InnerXML:
                    return node?.InnerXml;
                case ParserMapOutput.Value:
                    return node?.Value;
                default:
                    return node?.InnerText;
            }
        }

        public void ExportAsCsv(List<IDictionary<string, object>> data, string path)
        {
            Log.Debug("Exporting csv: " + path);

            var records = new List<dynamic>();

            foreach (Dictionary<string, object> row in data)
            {
                records.Add(row.BuildCsvObject());
            }

            CsvUtil.WriteRecords(records, path);

            Log.Info("Export csv completed.");
        }
    }
}
