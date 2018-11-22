using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServerRequestTracker.Usage.Services
{
    public interface IClientUsageStore
    {
        Task TrackAsync(ClientUsageRecord record);

        Task<IEnumerable<IAggregatedClientUsageRecordRead>> GetClientUsageRecordsAsync(string clientId,
            string grantType, (DateTime?, DateTime?) range);
    }
}