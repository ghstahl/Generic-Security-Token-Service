using System.Threading.Tasks;
namespace GenericSecurityTokenService.Functions
{
    /// <summary>
    /// This provides interfaces to functions.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Invokes the function.
        /// </summary>
        Task InvokeAsync();
    }
}
