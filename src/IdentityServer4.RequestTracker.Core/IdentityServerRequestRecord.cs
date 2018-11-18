using Microsoft.AspNetCore.Http;

namespace IdentityServer4RequestTracker
{
    public class IdentityServerRequestRecord
    {
        public string EndpointKey { get; set; }
        public string ClientId { get; set; }
        public HttpContext HttpContext { get; set; }

    }
}