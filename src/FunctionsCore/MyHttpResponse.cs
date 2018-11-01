using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FunctionsCore
{
    public class MyHttpResponse : HttpResponse
    {
        public MyHttpResponse(HttpRequest req)
        {
            Headers = new HeaderDictionary();
            foreach (var header in req.Headers)
            {
                Headers.Append(header.Key,header.Value);
            }

            Body = new MemoryStream();
        }
        public override void OnStarting(Func<object, Task> callback, object state)
        {
            
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            
        }

        public override void Redirect(string location, bool permanent)
        {
            
        }

        public override HttpContext HttpContext { get;  }
        public override int StatusCode { get; set; }
        public override IHeaderDictionary Headers { get; }
        public override Stream Body { get; set; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }
        public override IResponseCookies Cookies { get; }
        public override bool HasStarted { get; }
    }
}