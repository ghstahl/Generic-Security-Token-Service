using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Tenant.Core.Host;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Tenant.Core
{
  
    public class ServerRecord<TStartup>: IServerRecord where TStartup: class
    {
        private ILogger _logger;
        public string BaseUrl { get; set; }
        public PathString PathStringBaseUrl { get; set; }
        private string _functionAppDirectory;
        public ServerRecord(string tenant,string functionAppDirectory,string settingsPath,IConfiguration hostConfiguration, ILogger logger)
        {
            _tenant = tenant;
            _functionAppDirectory = functionAppDirectory;
            _settingsPath = settingsPath;
            _hostConfiguration = hostConfiguration;
            _logger = logger;
        }
       
        private TestServer _testServer;
        private string _settingsPath;
        private string _tenant;
        private IConfiguration _hostConfiguration;

        public TestServer TestServer
        {
            get
            {
                if (_testServer == null)
                {
      

                    var configSection = _hostConfiguration.GetSection($"TenantOptions:Overrides:{_tenant}");
                
                    var webHostBuilder = new WebHostBuilder()
                        .ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            var env = hostingContext.HostingEnvironment;
                            config.Add(new ChainedConfigurationSource()
                            {
                                Configuration = configSection
                            });
                        })
                        .ConfigureLogging((hostingContext, logging) =>
                        {
                            logging.ClearProviders();
                            logging.AddProvider(new TenantHostLoggerProvider(_tenant,_logger));
                        })
                        .UseContentRoot($"{_functionAppDirectory}/{_settingsPath}")
                        .UseStartup<TStartup>()
                        .ConfigureServices(s =>
                        s.AddSingleton<IStartupConfigurationService, NullStartupConfigurationService>());
                    var server = new TestServer(webHostBuilder);
                    _testServer = server;
                }
                return _testServer;
            }
        }
    }
}