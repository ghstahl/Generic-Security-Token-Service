using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace GenericSecurityTokenService.Modules
{
    public interface IFunctionHttpContextAccessor : IHttpContextAccessor
    {
        HttpResponseMessage HttpResponseMessage { get; set; }
        HttpRequestMessage HttpRequestMessage { get; set; }
    }
    public class MyHttpContextAccessor : IFunctionHttpContextAccessor
    {
        private static AsyncLocal<(string traceIdentifier, HttpContext context)> _httpContextCurrent 
            = new AsyncLocal<(string traceIdentifier, HttpContext context)>();
        private static AsyncLocal<HttpResponseMessage> _httpResponseMessageCurrent = new AsyncLocal<HttpResponseMessage>();
        private static AsyncLocal<HttpRequestMessage> _httpRequestMessageCurrent = new AsyncLocal<HttpRequestMessage>();
        public HttpContext HttpContext
        {
            get
            {
                var value = _httpContextCurrent.Value;
                // Only return the context if the stored request id matches the stored trace identifier
                // context.TraceIdentifier is cleared by HttpContextFactory.Dispose.
                return value.traceIdentifier == value.context?.TraceIdentifier ? value.context : null;
            }
            set
            {
                _httpContextCurrent.Value = (value?.TraceIdentifier, value);
            }
        }

        public HttpResponseMessage HttpResponseMessage {
            get
            {
                var value = _httpContextCurrent.Value;
                var httpResponseMessage = _httpResponseMessageCurrent.Value;
             
                // Only return the context if the stored request id matches the stored trace identifier
                // context.TraceIdentifier is cleared by HttpContextFactory.Dispose.
                return value.traceIdentifier == value.context?.TraceIdentifier ? httpResponseMessage : null;
            }
            set
            {
                _httpResponseMessageCurrent.Value = value;
            }
        }

        public HttpRequestMessage HttpRequestMessage
        {
            get
            {
                var value = _httpContextCurrent.Value;
                var httpRequestMessage = _httpRequestMessageCurrent.Value;

                // Only return the context if the stored request id matches the stored trace identifier
                // context.TraceIdentifier is cleared by HttpContextFactory.Dispose.
                return value.traceIdentifier == value.context?.TraceIdentifier ? httpRequestMessage : null;
            }
            set
            {
                _httpRequestMessageCurrent.Value = value;
            }
        }
    }
}