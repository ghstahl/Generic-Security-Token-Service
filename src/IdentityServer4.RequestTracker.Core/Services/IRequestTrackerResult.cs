using System.Threading.Tasks;
using IdentityServerRequestTracker.Models;
using Microsoft.AspNetCore.Http;

namespace IdentityServerRequestTracker.Services
{
    public interface IRequestTrackerResult
    {
        RequestTrackerEvaluatorDirective Directive { get; }
        Task ProcessAsync(HttpContext httpContext);
    }
}