using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer4Extras.Services
{
    public interface ITokenServiceHookPlugin
    {
        Task<(bool proccessed, Token token)> OnPostCreateAccessTokenAsync(TokenCreationRequest request, Token token);
        Task<(bool proccessed, Token token)> OnPostCreateIdentityTokenAsync(TokenCreationRequest request, Token token);
    }
}