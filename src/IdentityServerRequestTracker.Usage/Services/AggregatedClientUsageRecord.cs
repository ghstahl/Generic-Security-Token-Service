using System;

namespace IdentityServerRequestTracker.Usage.Services
{
    public class AggregatedClientUsageRecord : IAggregatedClientUsageRecordReadWrite
    {
        public (DateTime, DateTime) DateRange { get; set; }
        public string ClientId { get; set; }
        public string GrantType { get; set; }
        public string EndPointKey { get; set; }
        public int Count { get; set; }

    }
}