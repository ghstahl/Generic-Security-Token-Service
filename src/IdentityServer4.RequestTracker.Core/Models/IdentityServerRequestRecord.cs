using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;

namespace IdentityServerRequestTracker.Models
{
    public class IdentityServerRequestRecord
    {
        public string EndpointKey { get; set; }
        public HttpContext HttpContext { get; set; }
        public Client Client { get; set; }
        
    }
}