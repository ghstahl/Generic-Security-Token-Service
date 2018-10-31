using System.Threading;
using Microsoft.AspNetCore.Http;

namespace GenericSecurityTokenService.Modules
{
    public class MyHttpContextAccessor : IHttpContextAccessor
    {
        private static AsyncLocal<(string traceIdentifier, HttpContext context)> _httpContextCurrent 
            = new AsyncLocal<(string traceIdentifier, HttpContext context)>();

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
    }
}