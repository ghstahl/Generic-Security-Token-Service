using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServerRequestTracker.Usage.Services
{
    public class ClientUsageRecord
    {
        public DateTime DateTime { get; set; }
        public string ClientId { get; set; }
        public string GrantType { get; set; }
        public string EndPointKey { get; set; }
    }
    
    public class InMemoryClientUsageStore : IClientUsageStore
    {
        
        public InMemoryClientUsageStore()
        {
        }

        public async Task TrackAsync(ClientUsageRecord record)
        {
           
        }
    }
}