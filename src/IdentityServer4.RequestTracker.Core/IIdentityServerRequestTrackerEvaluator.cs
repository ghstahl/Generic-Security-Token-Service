using System.Threading.Tasks;

namespace IdentityServer4RequestTracker
{
    public interface IIdentityServerRequestTrackerEvaluator
    {
        string Name { get; set; }
        Task<RequestTrackerEvaluatorDirective> EvaluateAsync(IdentityServerRequestRecord requestRecord);
    }
}