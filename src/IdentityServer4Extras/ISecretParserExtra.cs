using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4Extras
{
    //
    // Summary:
    //     A service for parsing secrets found on the formCollection
    public interface ISecretParserExtra
    {
        //
        // Summary:
        //     Returns the authentication method name that this parser implements
        string AuthenticationMethod { get; }

        //
        // Summary:
        //     Tries to find a secret on the context that can be used for authentication
        //
        // Parameters:
        //   context:
        //     The formCollection.
        //
        // Returns:
        //     A parsed secret
        Task<ParsedSecret> ParseAsync(IFormCollection formCollection);
    }
}