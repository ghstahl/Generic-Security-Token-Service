using Microsoft.AspNetCore.Http;

namespace GenericSecurityTokenService.Functions.FunctionOptions
{
    public class DefaultHttpTriggerOptions : FunctionOptionsBase
    {

        public DefaultHttpTriggerOptions(HttpRequest req)
        {
            HttpRequest = req;
        }
        public HttpRequest HttpRequest { get; }
    }
}