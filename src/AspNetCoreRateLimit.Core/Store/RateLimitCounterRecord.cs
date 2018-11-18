using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreRateLimit.Core.Store
{
    public class RateLimitCounterRecord
    {
        public RateLimitCounter? RateLimitCounter { get; set; }
        public RateLimitRule RateLimitRule { get; set; }
    }
}
