using System;
using System.Collections.Concurrent;
using System.IO;
using GenericSecurityTokenService.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace GenericSecurityTokenService
{
    public class MyLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
    }

    public class MyLoggerProvider : ILoggerProvider
    {

        private readonly MyLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();
        private static ILogger _logger;
        public MyLoggerProvider(ILogger logger)
        {
            _logger = logger;
        }
        public MyLoggerProvider(MyLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => _logger);

        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }

    public static class TheHost
    {
        private static TestServer Server;

        public static TestServer GetServer(ExecutionContext context, Microsoft.AspNetCore.Http.HttpRequest req, ILogger log)
        {
            try
            {
                if (Server == null)
                {
                    
                    log.LogInformation("Creating TestServer");
                    var webHostBuilder = new WebHostBuilder()
                        .ConfigureLogging((hostingContext, logging) =>
                        {
                            logging.ClearProviders();
                            logging.AddProvider(new MyLoggerProvider(log));
                        })
                        .UseContentRoot(context.FunctionAppDirectory)
                        .UseStartup<Startup>();
                   webHostBuilder.ConfigureServices(s => s.AddSingleton<IStartupConfigurationService, StartupConfigurationService>());

                    Server = new TestServer(webHostBuilder);
                    var baseAddress =  req.Scheme + "://" + req.Host.ToUriComponent();
                    Server.BaseAddress = new Uri(baseAddress);
                }
                return Server;
            }
            catch (Exception e)
            {
                log.LogError(e,$"Creating Server Exception:{e.Message}");
                throw;
            }
        }
    }
}
