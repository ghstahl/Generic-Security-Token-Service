using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using IdentityServer4Extras.Endpoints;
using IdentityServer4Extras.Stores;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using P7.Core.Cache;

namespace GlorifiedJwt.Controllers
{
    class CustomPayload
    {
        public string A { get; set; }
        public string B { get; set; }
        public List<string> CList => new List<string>() {"a", "b", "c"};
    }
    [Route("api/auth")]
    [ApiController]
    public class BindController : ControllerBase
    {
        private ILogger<BindController> _logger;
        private ITokenEndpointHandlerExtra _tokenEndpointHandlerExtra;

        public BindController(
            ITokenEndpointHandlerExtra tokenEndpointHandlerExtra,
            ILogger<BindController> logger)
        {
            _tokenEndpointHandlerExtra = tokenEndpointHandlerExtra;
            _logger = logger;
        }
        [HttpPost]
        [Route("revocation")]
        public async Task<TokenRawResult> PostRevocationAsync()
        {
            /*
             *TokenTypHint: [refresh_token,subject,access_token]
             */
            var arbResourceOwnerResult = await PostRefreshAsync();
            var revocationRequest = new RevocationRequest()
            {
                Token = arbResourceOwnerResult.TokenResult.Response.RefreshToken,
                ClientId = "arbitrary-resource-owner-client",
                TokenTypHint = "refresh_token", 
                RevokeAllSubjects = "true"
            };
            var revocationResult = await _tokenEndpointHandlerExtra.ProcessRawAsync(revocationRequest);
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                RefreshToken = arbResourceOwnerResult.TokenResult.Response.RefreshToken,
                ClientId = "arbitrary-resource-owner-client"
            };
            arbResourceOwnerResult = await _tokenEndpointHandlerExtra.ProcessRawAsync(refreshTokenRequest);
            return arbResourceOwnerResult;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<TokenRawResult> PostRefreshAsync()
        {
            var arbResourceOwnerResult = await PostArbitraryResourceOwnerAsync();
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                RefreshToken = arbResourceOwnerResult.TokenResult.Response.RefreshToken,
                ClientId = "arbitrary-resource-owner-client"
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(refreshTokenRequest);
            return result;
        }
        
        [HttpPost]
        [Route("arbitrary_no_subject")]
        public async Task<TokenRawResult> PostArbitraryNoSubjectAsync()
        {
            _logger.LogInformation("Summary Executing...");
            var extensionGrantRequest = new ArbitraryNoSubjectRequest()
            {
                ClientId = "arbitrary-resource-owner-client",
                Scopes = new List<string>()
                {
                    "metal", "nitro", "In", "Flames"
                },
                ArbitraryClaims = new Dictionary<string, List<string>>()
                {
                    {"top",new List<string>(){"dog"}},
                    {"role",new List<string>(){"application","limited"}},
                    {"query",new List<string>(){ "dashboard", "licensing"}},
                    {"seatId",new List<string>(){"2368d213-d06c-4c2a-a099-11c34adc357"}},
                    {"piid",new List<string>(){"2368d213-1111-4c2a-a099-11c34adc3579"}}
                },
                AccessTokenLifetime = "3600",
                ArbitraryAmrs = new List<string>()
                {
                    "agent:username:agent0@supporttech.com",
                    "agent:challenge:fullSSN",
                    "agent:challenge:homeZip"
                },
                ArbitraryAudiences = new List<string>() { "cat", "dog" },
                CustomPayload = new CustomPayload()
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(extensionGrantRequest);
            return result;
        }
      
        [HttpPost]
        [Route("arbitrary_resource_owner")]
        public async Task<TokenRawResult> PostArbitraryResourceOwnerAsync()
        {
            _logger.LogInformation("Summary Executing...");
            var extensionGrantRequest = new ArbitraryResourceOwnerRequest()
            {
                ClientId = "arbitrary-resource-owner-client",
                Scopes = new List<string>()
                {
                    "offline_access", "metal", "nitro", "In", "Flames"
                },
                Subject = "PorkyPig",
                ArbitraryClaims = new Dictionary<string, List<string>>()
                {
                    {"top",new List<string>(){"dog"}},
                    {"role",new List<string>(){"application","limited"}},
                    {"query",new List<string>(){ "dashboard", "licensing"}},
                    {"seatId",new List<string>(){"2368d213-d06c-4c2a-a099-11c34adc357"}},
                    {"piid",new List<string>(){"2368d213-1111-4c2a-a099-11c34adc3579"}}
                },
                AccessTokenLifetime = "3600",
                ArbitraryAmrs = new List<string>()
                {
                    "agent:username:agent0@supporttech.com",
                    "agent:challenge:fullSSN",
                    "agent:challenge:homeZip"
                },
                ArbitraryAudiences = new List<string>() { "cat","dog" },
                CustomPayload = new CustomPayload()
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(extensionGrantRequest);
            return result;
        }
      
        [HttpPost]
        [Route("arbitrary_identity")]
        public async Task<TokenRawResult> PostArbitraryIdentityAsync()
        {
            _logger.LogInformation("Summary Executing...");
            var extensionGrantRequest = new ArbitraryIdentityRequest()
            {
                ClientId = "arbitrary-resource-owner-client",
                Scopes = new List<string>()
                {
                    "offline_access", "metal", "nitro", "In", "Flames"
                },
                Subject = "PorkyPig",
                ArbitraryClaims = new Dictionary<string, List<string>>()
                {
                    {"top",new List<string>(){"dog"}},
                    {"role",new List<string>(){"application","limited"}},
                    {"query",new List<string>(){ "dashboard", "licensing"}},
                    {"seatId",new List<string>(){"2368d213-d06c-4c2a-a099-11c34adc357"}},
                    {"piid",new List<string>(){"2368d213-1111-4c2a-a099-11c34adc3579"}}
                },
                AccessTokenLifetime = "3600",
                ArbitraryAmrs = new List<string>()
                {
                    "agent:username:agent0@supporttech.com",
                    "agent:challenge:fullSSN",
                    "agent:challenge:homeZip"
                },
                ArbitraryAudiences = new List<string>() { "cat", "dog" },
                CustomPayload = new CustomPayload()
            };
            var result = await _tokenEndpointHandlerExtra.ProcessRawAsync(extensionGrantRequest);
            return result;
        }
    }
}