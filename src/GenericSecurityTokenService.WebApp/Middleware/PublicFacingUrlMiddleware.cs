using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericSecurityTokenService.Controllers;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using P7.Core.Cache;

namespace GenericSecurityTokenService.Middleware
{
    
    /// <summary>
    /// Configures the HttpContext by assigning IdentityServerOrigin.
    /// </summary>
    public class PublicFacingUrlMiddleware
    {
        public static string PathRootUrl { get; set; }
        private readonly RequestDelegate _next;
     
        private ISingletonObjectCache<PublicFacingUrlMiddleware, Dictionary<string, object>> _objectCache;

        public PublicFacingUrlMiddleware(RequestDelegate next,
            ISingletonObjectCache<PublicFacingUrlMiddleware, Dictionary<string, object>> objectCache )
        {
            _next = next;
            _objectCache = objectCache;
        }

        private static string GetIdentityServerOrigin(HttpContext context)
        {
            var request = context.Request;
            var origin = $"{request.Scheme}://{request.Host}";
            if (!string.IsNullOrEmpty(PathRootUrl))
            {
                origin += $"/{PathRootUrl}";
            }
            return origin;
        }

      

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            context.SetIdentityServerOrigin(GetIdentityServerOrigin(context));
            context.SetIdentityServerBasePath(request.PathBase.Value.TrimEnd('/'));
            await _next(context);
        }
    }
    
}
