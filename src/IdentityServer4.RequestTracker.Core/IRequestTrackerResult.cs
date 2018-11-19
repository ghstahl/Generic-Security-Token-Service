using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityServerRequestTracker
{
    public interface IRequestTrackerResult
    {
        RequestTrackerEvaluatorDirective Directive { get; }
        Task ProcessAsync(HttpContext httpContext);
    }
}