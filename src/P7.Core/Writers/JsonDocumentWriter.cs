using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace P7.Core.Writers
{
    public interface IJsonDocumentWriterOptions
    {
        Formatting Formatting { get; }
        JsonSerializerSettings JsonSerializerSettings { get; }
    }

    public class JsonDocumentWriter
    {
        private readonly Formatting _formatting;
        private readonly JsonSerializerSettings _settings;
        public JsonSerializerSettings JsonSerializerSettings { get { return _settings;} }
        public JsonDocumentWriter()
            : this(indent: false)
        {
        }

        public JsonDocumentWriter(IJsonDocumentWriterOptions options)
            : this(options.Formatting,options.JsonSerializerSettings)
        {
        }

        public JsonDocumentWriter(bool indent)
            : this(
                indent ? Formatting.Indented : Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter>()
                    {
                        new Newtonsoft.Json.Converters.IsoDateTimeConverter()
                    }
                })
        {
        }

        public JsonDocumentWriter(Formatting formatting, JsonSerializerSettings settings)
        {
            _formatting = formatting;
            _settings = settings;
        }
        public string SerializeObjectSingleQuote<T>(T obj)
        {
            return SerializeObject<T>(obj, '\'');
        }

        public string SerializeObject<T>(T obj)
        {
            return SerializeObject<T>(obj, '\"');
        }

        public string SerializeObject<T>(T obj, char quoteChar)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (JsonTextWriter writer = new JsonTextWriter(sw))
            {
                writer.QuoteChar = quoteChar;

                JsonSerializer ser = new JsonSerializer
                {
                    ContractResolver = _settings.ContractResolver,
                    DateFormatHandling = _settings.DateFormatHandling,
                    DateFormatString = _settings.DateFormatString,
                    Formatting = _formatting,
                    DefaultValueHandling = _settings.DefaultValueHandling,
                    NullValueHandling = _settings.NullValueHandling
                };

                ser.Serialize(writer, obj);
            }
            return sb.ToString();
        }

    }
}
