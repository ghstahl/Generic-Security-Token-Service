using System.Threading.Tasks;

namespace IdentityServerRequestTracker
{
    public interface IIdentityServerRequestTrackerEvaluator
    {
        string Name { get; set; }
        Task<IRequestTrackerResult> EvaluateAsync(IdentityServerRequestRecord requestRecord);
    }
}