using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreRateLimit.Core.Store
{
    public interface IClientRateLimitCounterStore
    {
        Task<IEnumerable<RateLimitCounterRecord>> GetRateLimitCounterRecordsAsync(ClientRequestIdentity clientRequestIdentity);
    }
}