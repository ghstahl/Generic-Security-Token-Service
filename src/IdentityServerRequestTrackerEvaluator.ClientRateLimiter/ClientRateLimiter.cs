using System;
using System.Threading.Tasks;
using IdentityServer4RequestTracker;
using IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter
{
    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)  
        {
            var result = (T)serviceProvider.GetService(typeof(T));
            return result;
        }
    }
    public class ClientRateLimiterRequestTrackerResult : IRequestTrackerResult
    {
        private IServiceProvider _serviceProvider;
        private ClientRateLimitingOptions _options;
        public RequestTrackerEvaluatorDirective Directive { get; set; }

        public ClientRateLimiterRequestTrackerResult(
            IOptions<ClientRateLimitingOptions> options,
            IServiceProvider serviceProvider)
        {
            _options = options.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task ProcessAsync(HttpContext httpContext)
        {
            var result = _serviceProvider.GetService(typeof(ClientRateLimiterRequestTrackerResult));
            var identity = new ClientRequestIdentity
            {
                Path = httpContext.Request.Path.ToString().ToLowerInvariant(),
                HttpVerb = httpContext.Request.Method.ToLowerInvariant(),
                ClientId = parsedSecret.Id
            };
            return identity;
        }

        
    }
    internal class ClientRateLimiter: IIdentityServerRequestTrackerEvaluator
    {
        private IServiceProvider _serviceProvider;

        public ClientRateLimiter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Name = "client_rate_limiter";
        }
        public string Name { get; set; }
        public async Task<IRequestTrackerResult> EvaluateAsync(IdentityServerRequestRecord requestRecord)
        {
            var result = _serviceProvider.GetService<ClientRateLimiterRequestTrackerResult>();
            result.Directive = RequestTrackerEvaluatorDirective.AllowRequest;
            return result;
        }

        
    }
}
