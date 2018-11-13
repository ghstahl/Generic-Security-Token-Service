using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using IdentityServer4Extras.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ArbitraryNoSubjectExtensionGrant
{
    public class ArbitraryNoSubjectRequestValidator
    {
        private readonly ILogger<ArbitraryNoSubjectRequestValidator> _logger;

        private static List<string> _requiredArbitraryArguments;

        private static List<string> RequiredArbitraryArguments => _requiredArbitraryArguments ??
                                                                  (_requiredArbitraryArguments =
                                                                      new List<string>
                                                                      {
                                                                          "client_id",
                                                                          "client_secret",
                                                                          "arbitrary_claims"
                                                                      });

        private static List<string> _notAllowedArbitraryClaims;

        private static List<string> NotAllowedArbitraryClaims => _notAllowedArbitraryClaims ??
                                                                 (_notAllowedArbitraryClaims =
                                                                     new List<string>
                                                                     {
                                                                         "nudibranch_watermark",
                                                                         ClaimTypes.NameIdentifier,
                                                                         ClaimTypes.AuthenticationMethod,
                                                                         JwtClaimTypes.AccessTokenHash,
                                                                         JwtClaimTypes.Audience,
                                                                         JwtClaimTypes.AuthenticationMethod,
                                                                         JwtClaimTypes.AuthenticationTime,
                                                                         JwtClaimTypes.AuthorizedParty,
                                                                         JwtClaimTypes.AuthorizationCodeHash,
                                                                         JwtClaimTypes.ClientId,
                                                                         JwtClaimTypes.Expiration,
                                                                         JwtClaimTypes.IdentityProvider,
                                                                         JwtClaimTypes.IssuedAt,
                                                                         JwtClaimTypes.Issuer,
                                                                         JwtClaimTypes.JwtId,
                                                                         JwtClaimTypes.Nonce,
                                                                         JwtClaimTypes.NotBefore,
                                                                         JwtClaimTypes.ReferenceTokenId,
                                                                         JwtClaimTypes.SessionId,
                                                                         JwtClaimTypes.Subject,
                                                                         JwtClaimTypes.Scope,
                                                                         JwtClaimTypes.Confirmation,
                                                                         Constants.CustomPayload
                                                                     });

        private static List<string> _notAllowedScopes;

        private static List<string> NotAllowedScopes => _notAllowedScopes ??
                                                        (_notAllowedScopes =
                                                            new List<string>
                                                            {
                                                                "offline_access"
                                                            });

        public ArbitraryNoSubjectRequestValidator(
            ILogger<ArbitraryNoSubjectRequestValidator> logger)
        {
            _logger = logger;
        }

        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var raw = context.Result.ValidatedRequest.Raw;
            var rr = raw.AllKeys.ToDictionary(k => k, k => raw[(string) k]);
            var error = false;
            var los = new List<string>();

            if (rr.ContainsKey(JwtClaimTypes.Scope))
            {
                var scopes = rr[JwtClaimTypes.Scope].Split(' ');
                var invalidScopes = (from o in scopes
                    join p in NotAllowedScopes on o equals p into t
                    from od in t.DefaultIfEmpty()
                    where od != null
                    select od).ToList();
                if (invalidScopes.Any())
                {
                    // not allowed.
                    error = true;
                    foreach (var invalidScope in invalidScopes)
                    {
                        los.Add($"The scope: '{invalidScope}' is not allowed.");
                    }
                }
            }

            var result = RequiredArbitraryArguments.Except(rr.Keys);
            if (result.Any())
            {
                error = true;
                los.AddRange(result.Select(item => $"{item} is missing!"));

            }

            try
            {
                var arbitraryClaims = raw["arbitrary_claims"];
                if (!string.IsNullOrWhiteSpace(arbitraryClaims))
                {
                    var values =
                        JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(arbitraryClaims);
                    var invalidClaims = (from o in values
                        join p in NotAllowedArbitraryClaims on o.Key equals p into t
                        from od in t.DefaultIfEmpty()
                        where od != null
                        select od).ToList();
                    if (invalidClaims.Any())
                    {
                        // not allowed.
                        error = true;
                        foreach (var invalidClaim in invalidClaims)
                        {
                            los.Add($"The arbitrary claim: '{invalidClaim}' is not allowed.");
                        }

                    }
                }

            }
            catch (Exception _)
            {
                error = true;
                los.Add($"arbitrary_claims is malformed!");
            }
            if (!error)
            {
                var customPayload = raw[Constants.CustomPayload];
                if (!string.IsNullOrWhiteSpace(customPayload))
                {
                    error = !customPayload.IsValidJson();
                    if (error)
                    {
                        los.Add($"{Constants.CustomPayload} is not valid: '{customPayload}'.");
                    }
                }
            }
            if (error)
            {
                context.Result.IsError = true;
                context.Result.Error = String.Join<string>(" | ", los);
                ;
            }
        }
    }
}