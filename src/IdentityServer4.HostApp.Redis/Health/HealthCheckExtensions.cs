using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;


namespace IdentityServer4.HostApp.Health
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddMyHealthCheck(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<RedisHealthCheck>();
            services.AddTransient<KeyVaultHealthCheck>();
            
            var appConfig = new HealthCheckOptions();
            config.GetSection("healthCheck").Bind(appConfig);

            services.AddHealthChecks(checks =>
            {
                checks.AddCheck<RedisHealthCheck>("redis-health");
                checks.AddCheck<KeyVaultHealthCheck>("keyvault-health");

                /*
                checks.AddUrlCheck("https://github.com")
                    .AddHealthCheckGroup(
                        "servers",
                        group => group.AddUrlCheck("https://google.com")
                            .AddUrlCheck("https://twitddter.com")
                    )
                    .AddHealthCheckGroup(
                        "memory",
                        group => group.AddPrivateMemorySizeCheck(1)
                            .AddVirtualMemorySizeCheck(2)
                            .AddWorkingSetCheck(1),
                        CheckStatus.Unhealthy
                    )
                    .AddCheck("thrower", (Func<IHealthCheckResult>)(() => { throw new DivideByZeroException(); }))
                    .AddCheck("long-running", async cancellationToken => { await Task.Delay(10000, cancellationToken); return HealthCheckResult.Healthy("I ran too long"); })
                    .AddCheck<CustomHealthCheck>("custom");
                    */
                /*
                // add valid storage account credentials first
                checks.AddAzureBlobStorageCheck("accountName", "accountKey");
                checks.AddAzureBlobStorageCheck("accountName", "accountKey", "containerName");
                checks.AddAzureTableStorageCheck("accountName", "accountKey");
                checks.AddAzureTableStorageCheck("accountName", "accountKey", "tableName");
                checks.AddAzureFileStorageCheck("accountName", "accountKey");
                checks.AddAzureFileStorageCheck("accountName", "accountKey", "shareName");
                checks.AddAzureQueueStorageCheck("accountName", "accountKey");
                checks.AddAzureQueueStorageCheck("accountName", "accountKey", "queueName");
                */

            });
            return services;
        }
    }
}