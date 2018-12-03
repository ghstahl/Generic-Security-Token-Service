using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tenant.Core.AzureFunctions
{
    public static class TenantApps<TStartup> where TStartup: class
    {
        private static List<IServerRecord> _serversRecords;

        public static List<IServerRecord> GetServerRecords(
            string functionAppDirectory,
            IConfiguration configuration,
            ILogger logger)
        {
            if (_serversRecords == null)
            {
                var configSection = configuration.GetSection("tenantOptions");
                TenantOptions tenantOptions = new TenantOptions();
                configSection.Bind(tenantOptions);

                var serverRecords = new List<IServerRecord>();
                foreach (var tenant in tenantOptions.Tenants)
                {
                    serverRecords.Add(
                        new ServerRecord<TStartup>(
                            tenant.Name,
                            functionAppDirectory,
                            tenant.SettingsPath,
                            configuration,
                            logger)
                        {
                            BaseUrl = tenant.BaseUrl,
                            PathStringBaseUrl = new PathString(tenant.BaseUrl)
                        });
                }
                _serversRecords = serverRecords;
            }
            return _serversRecords;
        }
    }
}