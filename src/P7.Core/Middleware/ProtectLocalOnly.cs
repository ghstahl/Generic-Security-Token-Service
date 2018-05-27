using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P7.Core.Providers;
using P7.Core.Settings;

namespace P7.Core.Middleware
{
    public class ProtectLocalOnly
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<FiltersConfig> _settings;
        private readonly ILogger<OptOutOptInFilterProvider> _logger;
      
        public ProtectLocalOnly(RequestDelegate next, ProtectLocalOnlyOptions options, IOptions<FiltersConfig> settings, ILogger<OptOutOptInFilterProvider> logger)
        {
            _next = next;
            _settings = settings;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, IAuthorizationService authorizationService)
        {
            bool isLocal = true;
            if (httpContext.Connection.RemoteIpAddress != null)
            {
                if (string.CompareOrdinal("::1", httpContext.Connection.RemoteIpAddress.ToString()) != 0)
                {
                    isLocal = false;
                }
            }
     
            if (!isLocal)
            {
                var paths = _settings.Value.MiddleWare.ProtectLocalOnly.Paths;
                if (paths.Any(path => httpContext.Request.Path.StartsWithSegments(path)))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
            }
            await _next(httpContext);
        }
    }
}