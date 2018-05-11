using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Interface for the token request validator
    /// </summary>
    public interface IAuthorize2RequestValidator
    {
        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="clientValidationResult">The client validation result.</param>
        /// <returns></returns>
        Task<Authorize2RequestValidationResult> ValidateRequestAsync(NameValueCollection parameters, ClientSecretValidationResult clientValidationResult);
    }
}