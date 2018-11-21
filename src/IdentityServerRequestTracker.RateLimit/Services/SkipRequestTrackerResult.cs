using System.Threading.Tasks;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;

namespace IdentityServerRequestTracker.RateLimit.Services
{
    public class SkipRequestTrackerResult : IRequestTrackerResult
    {
        public RequestTrackerEvaluatorDirective Directive => RequestTrackerEvaluatorDirective.Skip;

        public Task ProcessAsync(HttpContext httpContext)
        {
            return Task.CompletedTask;
        }
    }
}