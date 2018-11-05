using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GenericSecurityTokenService.Middleware
{
    
    /// <summary>
    /// Configures the HttpContext by assigning IdentityServerOrigin.
    /// </summary>
    public class PublicFacingUrlMiddleware
    {
        public static string PathRootUrl { get; set; }
        private readonly RequestDelegate _next;
        private static string _identityServerOrigin;
        private static string GetIdentityServerOrigin(HttpContext context)
        {
            if (_identityServerOrigin == null)
            {
                var request = context.Request;
                var origin = $"{request.Scheme}://{request.Host}";
                if (!string.IsNullOrEmpty(PathRootUrl))
                {
                    origin += $"/{PathRootUrl}";
                }

                _identityServerOrigin = origin;
            }

            return _identityServerOrigin;
        }

        public PublicFacingUrlMiddleware(RequestDelegate next)
        {
            _next = next;
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
