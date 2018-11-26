using System;

namespace IdentityServerRequestTracker.Usage.Services
{
    public class ClientUsageRecord
    {
        public DateTime DateTime { get; set; }
        public string ClientId { get; set; }
        public string GrantType { get; set; }
        public string EndPointKey { get; set; }
    }
}