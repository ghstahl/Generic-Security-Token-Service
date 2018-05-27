using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace P7.Core
{
    public class Global
    {
        public static IHostingEnvironment HostingEnvironment { get; set; }
        public static IMemoryCache MemoryCache { get; set; }
        public static IConfiguration Configuration { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }

        private static Dictionary<string, object> _arbitraryObjects;
        public static Dictionary<string, object> ArbitraryObjects
        {
            get { return _arbitraryObjects ?? (_arbitraryObjects = new Dictionary<string, object>()); }
        }
    }
}
