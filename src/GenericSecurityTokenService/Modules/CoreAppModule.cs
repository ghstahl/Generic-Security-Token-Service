using System.IO;
using System.Net.Http;
using GenericSecurityTokenService.Functions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenericSecurityTokenService.Modules
{
    /// <summary>
    /// This represents the module entity for dependencies.
    /// </summary>
    public class CoreAppModule : Module
    {
        private string _basePath;
        public CoreAppModule(string basePath)
        {
            _basePath = basePath;
        }
        /// <inheritdoc />
        public override void Load(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(_basePath)
                .AddJsonFile("config.json")
                .Build();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHelloFunction, CoreHelloFunction>();
        }
    }
}
