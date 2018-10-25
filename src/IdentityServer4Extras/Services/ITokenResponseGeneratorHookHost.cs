using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace IdentityServer4Extras.Services
{
    public interface ITokenResponseGeneratorHookHost
    {
        Task<(string accessToken, string refreshToken)> CreateAccessTokenAsync(ValidatedTokenRequest request);
    }
}