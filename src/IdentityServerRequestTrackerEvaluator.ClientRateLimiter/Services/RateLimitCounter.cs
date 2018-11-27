﻿using System;

namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Services
{
    /// <summary>
    /// Stores the initial access time and the numbers of calls made from that point
    /// </summary>
    public struct RateLimitCounter
    {
        public DateTime Timestamp { get; set; }

        public long TotalRequests { get; set; }
    }
}