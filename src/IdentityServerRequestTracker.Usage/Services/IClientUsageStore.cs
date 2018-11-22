using System.Threading.Tasks;

namespace IdentityServerRequestTracker.Usage.Services
{
    public interface IClientUsageStore
    {
        Task TrackAsync(ClientUsageRecord record);
    }
}