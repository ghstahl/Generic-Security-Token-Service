using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace P7.Core
{
    public class GlobalConfigurationRoot : IConfiguration
    {
        public static IConfiguration Configuration { get; set; }
        public IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return Configuration.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            return Configuration.GetReloadToken();
        }

        public string this[string key]
        {
            get => Configuration[key];
            set => Configuration[key] = value;
        }
    }
}
