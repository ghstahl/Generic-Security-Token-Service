using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P7IdentityServer4;
using P7IdentityServer4.Extensions;

namespace ConsoleKeyVault
{
    class Program
    {
        public static ServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.keyVault.json", true, true)
                .AddJsonFile("logging.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();

            var services = new ServiceCollection()
                    .AddLogging(builder =>
                    {
                        builder
                            .AddConfiguration(Configuration.GetSection("Logging"))
                            .AddFilter("Microsoft", LogLevel.Warning)
                            .AddFilter("System", LogLevel.Warning)
                            .AddFilter("SampleApp.Program", LogLevel.Debug)
                            .AddConsole();
                    })
                .AddKeyVaultTokenCreateServiceConfiguration(Configuration)
                .AddKeyVaultTokenCreateServiceTypes()
                .AddMemoryCache();
            ServiceProvider = services.BuildServiceProvider();
            //configure console logging
            ServiceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = ServiceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            var signingCredentialStore =
                ServiceProvider.GetService<ITokenSigningCredentialStore>();
            var x509Certificate2 = await signingCredentialStore.GeX509Certificate2Async();
            logger.LogDebug("All done!");
        }
    }
}
