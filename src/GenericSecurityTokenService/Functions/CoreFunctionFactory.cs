using System;
using GenericSecurityTokenService.Containers;
using GenericSecurityTokenService.Modules;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GenericSecurityTokenService.Functions
{
    /// <summary>
    /// This represents the factory entity for functions.
    /// </summary>
    public class CoreFunctionFactory : IFunctionFactory
    {
        private readonly IServiceProvider _container;

        public IServiceProvider ServiceProvider => _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreFunctionFactory"/> class.
        /// </summary>
        /// <param name="module"><see cref="IModule"/> instance.</param>
        public CoreFunctionFactory(IModule module = null)
        {
            this._container = new ContainerBuilder()
                .RegisterModule(module)
                .Build();
        }

        /// <inheritdoc />
        public TFunction Create<TFunction>(ILogger log, string name = null)
            where TFunction : IFunction
        {
            // Resolve the function instance directly from the container.
            var function = this._container.GetService<TFunction>();
            function.Log = log;

            return function;
        }

    }
}