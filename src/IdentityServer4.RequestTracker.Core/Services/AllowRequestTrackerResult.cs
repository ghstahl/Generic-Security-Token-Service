using System.Threading.Tasks;
using IdentityServerRequestTracker.Models;
using Microsoft.AspNetCore.Http;

namespace IdentityServerRequestTracker.Services
{
    public class AllowRequestTrackerResult : IAllowRequestTrackerResult
    {
        public RequestTrackerEvaluatorDirective Directive => RequestTrackerEvaluatorDirective.AllowRequest;

        public Task ProcessAsync(HttpContext httpContext)
        {
            return Task.CompletedTask;
        }
    }
}