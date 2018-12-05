using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4Extras.Endpoints
{
    public class ExtensionGrantRequest
    {
        public string ClientId { get; set; }
        public string GrantType { get; set; }
        public List<string> Scopes { get; set; }
        public string Subject { get; set; }
        public string AccessTokenLifetime { get; set; }
        public Dictionary<string, List<string>> ArbitraryClaims { get; set; }
        public List<string> ArbitraryAmrs { get; set; }
        public List<string> ArbitraryAudiences { get; set; }
        public object CustomPayload { get; set; }

    }
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
        Task<IEndpointResult> ProcessAsync(ExtensionGrantRequest extensionGrantRequest);
    }
}