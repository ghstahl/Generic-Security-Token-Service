using System;
using System.Threading.Tasks;
using IdentityServer4RequestTracker;
using Microsoft.AspNetCore.Http;

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
        public RequestTrackerEvaluatorDirective Directive { get; set; }

        public ClientRateLimiterRequestTrackerResult(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ProcessAsync(HttpContext httpContext)
        {
            var result = _serviceProvider.GetService(typeof(ClientRateLimiterRequestTrackerResult));
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
        public async Task<RequestTrackerEvaluatorDirective> EvaluateAsync(IdentityServerRequestRecord requestRecord)
        {
            return RequestTrackerEvaluatorDirective.AllowRequest;
        }

        Task<IRequestTrackerResult> IIdentityServerRequestTrackerEvaluator.EvaluateAsync(IdentityServerRequestRecord requestRecord)
        {
            var result = _serviceProvider.GetService< ClientRateLimiterRequestTrackerResult>();
            result.Directive = RequestTrackerEvaluatorDirective.AllowRequest;
            throw new NotImplementedException();
        }
    }
}
