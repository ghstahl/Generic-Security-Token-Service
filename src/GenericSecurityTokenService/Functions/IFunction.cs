using System.Threading.Tasks;
using GenericSecurityTokenService.Functions.FunctionOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GenericSecurityTokenService.Functions
{
    /// <summary>
    /// This provides interfaces to functions.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Gets or sets the <see cref="TraceWriter"/> instance.
        /// </summary>
        ILogger Log { get; set; }

        /// <summary>
        /// Invokes the function.
        /// </summary>
        /// <typeparam name="TInput">Type of input.</typeparam>
        /// <typeparam name="TOutput">Type of output.</typeparam>
        /// <param name="input">Input instance.</param>
        /// <param name="options"><see cref="FunctionOptionsBase"/> instance.</param>
        /// <returns>Returns output instance.</returns>
        Task<ActionResult> InvokeAsync<TInput>(TInput input, FunctionOptionsBase options);
    }
}
