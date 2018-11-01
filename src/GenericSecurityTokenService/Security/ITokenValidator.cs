using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GenericSecurityTokenService.Security
{
    public interface ITokenValidator
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue value);
    }
}