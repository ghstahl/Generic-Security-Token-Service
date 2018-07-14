using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.HostApp.Options
{
    public class RedisAppOptions
    {
        public bool UseRedis { get; set; }
        public string RedisConnectionString { get; set; }
    }
}
