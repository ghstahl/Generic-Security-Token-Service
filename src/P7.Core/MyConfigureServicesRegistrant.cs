using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using P7.Core.Settings;

namespace P7.Core
{
    public static class ConfigurationServicesExtension
    {
        public static void RegisterP7CoreConfigurationServices(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.Configure<FiltersConfig>(configuration.GetSection(FiltersConfig.WellKnown_SectionName));
        }
    }
 
}
