using System.Threading.Tasks;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4Extras.Endpoints
{
    public interface ITokenEndpointHandlerExtra
    {
        //
        // Summary:
        //     Processes the request.
        //
        // Parameters:
        //   context:
        //     The formCollection.
        Task<IEndpointResult> ProcessAsync(IFormCollection formCollection);
        Task<TokenRawResult> ProcessRawAsync(IFormCollection formCollection);
    }
}