using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FunctionsCore.Security
{
    public interface ITokenValidator
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue value);
    }
}