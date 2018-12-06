using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4Extras.Endpoints
{
    public class ArbitraryResourceOwnerRequest
    {
        public string ClientId { get; set; }
        public List<string> Scopes { get; set; }
        public string Subject { get; set; }
        public string AccessTokenLifetime { get; set; }
        public Dictionary<string, List<string>> ArbitraryClaims { get; set; }
        public List<string> ArbitraryAmrs { get; set; }
        public List<string> ArbitraryAudiences { get; set; }
        public object CustomPayload { get; set; }
    }
    public class ArbitraryNoSubjectRequest
    {
        public string ClientId { get; set; }
        public List<string> Scopes { get; set; }
        public string AccessTokenLifetime { get; set; }
        public Dictionary<string, List<string>> ArbitraryClaims { get; set; }
        public List<string> ArbitraryAmrs { get; set; }
        public List<string> ArbitraryAudiences { get; set; }
        public object CustomPayload { get; set; }
    }
    public class ArbitraryIdentityRequest
    {
        public string ClientId { get; set; }
        public List<string> Scopes { get; set; }
        public string Subject { get; set; }
        public string AccessTokenLifetime { get; set; }
        public Dictionary<string, List<string>> ArbitraryClaims { get; set; }
        public List<string> ArbitraryAmrs { get; set; }
        public List<string> ArbitraryAudiences { get; set; }
        public object CustomPayload { get; set; }
    }
    public class RevocationRequest
    {
        public string ClientId { get; set; }
        public string TokenTypHint { get; set; }
        public string Token { get; set; }
        public string RevokeAllSubjects { get; set; }
    }
    public class RefreshTokenRequest
    {
        public string ClientId { get; set; }
        public string GrantType { get; set; }
        public string RefreshToken { get; set; }
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
        Task<IEndpointResult> ProcessAsync(ArbitraryResourceOwnerRequest request);
        Task<IEndpointResult> ProcessAsync(ArbitraryNoSubjectRequest request);
        Task<IEndpointResult> ProcessAsync(ArbitraryIdentityRequest request);
        Task<IEndpointResult> ProcessAsync(RefreshTokenRequest request);
        Task<IEndpointResult> ProcessAsync(RevocationRequest request);
    }
}