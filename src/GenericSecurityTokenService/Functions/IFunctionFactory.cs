using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GenericSecurityTokenService.Functions
{
    /// <summary>
    /// This provides interfaces to the <see cref="CoreFunctionFactory"/> instance.
    /// </summary>
    public interface IFunctionFactory
    {
        /// <summary>
        /// Creates a function instance from the IoC container.
        /// </summary>
        /// <typeparam name="TFunction">Type of function.</typeparam>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <param name="name">Instance name.</param>
        /// <returns>Returns the function instance.</returns>
        TFunction Create<TFunction>(ILogger log, string name = null) where TFunction : IFunction;
        IServiceProvider ServiceProvider { get; }
    }
}