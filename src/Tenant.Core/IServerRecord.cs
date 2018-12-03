using Microsoft.AspNetCore.Http;
using Tenant.Core.Host;

namespace Tenant.Core
{
    public interface IServerRecord
    {
        TestServer TestServer { get; }
        string BaseUrl { get; }
        PathString PathStringBaseUrl { get; set; }
    }
}