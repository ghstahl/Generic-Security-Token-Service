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
        /// <returns>Returns the function instance.</returns>
        TFunction Create<TFunction>() where TFunction : IFunction;
        IServiceProvider ServiceProvider { get; }
    }
}