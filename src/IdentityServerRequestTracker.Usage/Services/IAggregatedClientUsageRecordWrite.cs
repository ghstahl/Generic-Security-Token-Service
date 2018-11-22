using System;

namespace IdentityServerRequestTracker.Usage.Services
{
    public interface IAggregatedClientUsageRecordWrite
    {
        (DateTime, DateTime) DateRange { get; set; }
        string ClientId { get; set; }
        string GrantType { get; set; }
        string EndPointKey { get; set; }
        int Count { get; set; }
    }
}