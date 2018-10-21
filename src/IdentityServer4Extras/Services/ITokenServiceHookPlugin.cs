using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer4Extras.Services
{
    public interface ITokenServiceHookPlugin
    {
        Task<(bool, Token)> OnPostCreateAccessTokenAsync(TokenCreationRequest request, Token token);
        Task<(bool, Token)> OnPostCreateIdentityTokenAsync(TokenCreationRequest request, Token token);
    }
}