using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimpleSerilog.Extensions
{
    public static class JsonExtensions
    {
        private static string NULL = "null";

        public static string ToJson(this object entity)
        {
            if (entity == null)
                return NULL;

            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, entity);
                return writer.ToString();
            }
        }

        public static string ToJson(this object entity, Formatting formatting)
        {
            if (entity == null)
                return NULL;

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = formatting;

            return JsonConvert.SerializeObject(entity, settings);
        }
    }
}
