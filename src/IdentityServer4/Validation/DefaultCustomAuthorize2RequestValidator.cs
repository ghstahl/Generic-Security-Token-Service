using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Default custom request validator
    /// </summary>
    internal class DefaultCustomAuthorize2RequestValidator : ICustomAuthorize2RequestValidator
    {
        /// <summary>
        /// Custom validation logic for a token request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The validation result
        /// </returns>
 
        public Task ValidateAsync(CustomAuthorize2RequestValidationContext context)
        {
            return Task.CompletedTask;
        }
    }
}