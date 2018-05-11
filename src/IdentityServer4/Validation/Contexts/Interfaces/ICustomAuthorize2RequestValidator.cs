using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Allows inserting custom validation logic into authorize2  
    /// </summary>
    public interface ICustomAuthorize2RequestValidator
    {
        /// <summary>
        /// Custom validation logic for a token request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The validation result
        /// </returns>
        Task ValidateAsync(CustomAuthorize2RequestValidationContext context);
    }
}