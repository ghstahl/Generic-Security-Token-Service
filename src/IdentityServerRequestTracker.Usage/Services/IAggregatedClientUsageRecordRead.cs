using System;

namespace IdentityServerRequestTracker.Usage.Services
{
    public interface IAggregatedClientUsageRecordRead
    {
        (DateTime, DateTime) DateRange { get; }
        string ClientId { get; }
        string GrantType { get; }
        string EndPointKey { get; }
        int Count { get; }
    }
}