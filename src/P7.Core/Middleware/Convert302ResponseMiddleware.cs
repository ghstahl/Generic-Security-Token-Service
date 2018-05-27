using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace P7.Core.Middleware
{
    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
    class MetaData
    {
        public string Type { get; set; }
    }
    class Payload
    {
        private Dictionary<string, string> _headers;

        public Dictionary<string, string> Headers => _headers ?? (_headers = new Dictionary<string, string>());

        public MetaData MetaData { get; set; }
    }

    public class Convert302ResponseMiddleware
    {
        private readonly RequestDelegate _next;
        public Convert302ResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 302
                && context.Request.Headers.ContainsKey("X-302to200"))
            {
                context.Response.StatusCode = 200;
                Payload payload = new Payload();
               
                foreach (var header in context.Response.Headers)
                {
                    payload.Headers.Add(header.Key,header.Value);
                }
                payload.MetaData = new MetaData()
                {
                    Type = "X-302to200"
                };

                string json = JsonConvert.SerializeObject(payload, Converter.Settings);
                await context.Response.WriteAsync(json);
            }
        }
    }
}