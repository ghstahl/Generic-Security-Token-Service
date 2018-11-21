using System.Threading.Tasks;
using IdentityServerRequestTracker.Models;

namespace IdentityServerRequestTracker.Services
{
    public interface IIdentityServerRequestTrackerEvaluator
    {
        string Name { get; set; }
        Task<IRequestTrackerResult> PreEvaluateAsync(IdentityServerRequestRecord requestRecord);
        Task<IRequestTrackerResult> PostEvaluateAsync(IdentityServerRequestRecord requestRecord, bool error);
    }
}