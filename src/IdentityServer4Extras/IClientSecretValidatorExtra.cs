using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4Extras
{
    public interface IClientSecretValidatorExtra
    {
        //
        // Summary:
        //     Tries to authenticate a client based on the incoming request
        //
        // Parameters:
        //   context:
        //     The formCollection.
        Task<ClientSecretValidationResult> ValidateAsync(IFormCollection formCollection);
    }
}