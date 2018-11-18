using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4RequestTracker
{
    public interface IRequestTrackerResult
    {
        RequestTrackerEvaluatorDirective Directive { get; }
        Task ProcessAsync(HttpContext httpContext);
    }
}