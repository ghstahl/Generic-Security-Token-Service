using System;
using FunctionsCore.Containers;
using FunctionsCore.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionsCore.Functions
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
        public TFunction Create<TFunction>()
            where TFunction : IFunction
        {
            // Resolve the function instance directly from the container.
            var function = this._container.GetService<TFunction>();
            return function;
        }
    }
}