using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace SftpDataExport.Config
{
    public class ParserSettings
    {
        public List<ParserMapping> Mapping { get; set; } = new List<ParserMapping>();
    }

    public class ParserMapping
    {
        public string Name { get; set; }

        public bool IsLineItems { get; set; }

        public string LineItemPath { get; set; }

        public List<ParserMap> Map { get; set; } = new List<ParserMap>();
    }

    public class ParserMap
    {
        public string Name { get; set; }

        public string XPath { get; set; }

        /// <summary>
        /// Output Attribute, mandatory if Output = AttributeValue
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// Default InnerText
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ParserMapOutput Output { get; set; } = ParserMapOutput.InnerText;

        public bool IsMandatory { get; set; }

        public bool IsRootXPath { get; set; }
    }

    public enum ParserMapOutput
    {
        InnerText,
        InnerXML,
        Value,
        Attribute
    }
}
