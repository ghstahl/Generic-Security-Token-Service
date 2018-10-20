using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ArbitraryIdentityExtensionGrant
{
    static class RequestValidationExtensions
    {
        public static bool ValidateFormat<T>(this List<string> errorList, string name, string json)
        {
            bool error = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var values =
                        JsonConvert.DeserializeObject<T>(json);
                }

            }
            catch (Exception)
            {
                error = true;
                errorList.Add($"{name} is malformed!");
            }

            return error;
        }
    }

    public class ArbitraryIdentityRequestValidator
    {
 
        private readonly ILogger _logger;

        private static List<string> _requiredArbitraryArguments;
        private static List<string> RequiredArbitraryArguments => _requiredArbitraryArguments ??
                                                                  (_requiredArbitraryArguments =
                                                                      new List<string>
                                                                      {
                                                                          "client_id",
                                                                          "client_secret",
                                                                          Constants.ArbitraryClaims
                                                                      });
        private static List<string> _notAllowedArbitraryClaims;
        private static List<string> NotAllowedArbitraryClaims => _notAllowedArbitraryClaims ??
                                                                 (_notAllowedArbitraryClaims =
                                                                     new List<string>
                                                                     {
                                                                         "nudibranch_watermark",
                                                                         "client_id",
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
                                                                         JwtClaimTypes.Confirmation
                                                                     });

        private static List<string> _oneMustExitsArguments;
        private static List<string> OneMustExitsArguments => _oneMustExitsArguments ??
                                                                  (_oneMustExitsArguments =
                                                                      new List<string>
                                                                      {
                                                                          "subject",
                                                                          "access_token"
                                                                      });

        public ArbitraryIdentityRequestValidator(
            ILogger<ArbitraryIdentityRequestValidator> logger)
        {
            _logger = logger;
        }
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {

            var raw = context.Result.ValidatedRequest.Raw;
            var rr = raw.AllKeys.ToDictionary(k => k, k => raw[(string)k]);
            var error = false;
            var los = new List<string>();

            var oneMustExistResult = (from item in OneMustExitsArguments
                where rr.Keys.Contains(item)
                select item).ToList();

            if (!oneMustExistResult.Any())
            {
                error = true;
                los.AddRange(OneMustExitsArguments.Select(item => $"[one or the other] {item} is missing!"));
            }
            var result = RequiredArbitraryArguments.Except(rr.Keys);
            if (result.Any())
            {
                error = true;
                los.AddRange(result.Select(item => $"{item} is missing!"));

            }
            // make sure nothing is malformed
            error = los.ValidateFormat<Dictionary<string, List<string>>>(Constants.ArbitraryAmrs, raw[Constants.ArbitraryClaims]) || error;
            error = los.ValidateFormat<List<string>>(Constants.ArbitraryAmrs, raw[Constants.ArbitraryAmrs]) || error;
            error = los.ValidateFormat<List<string>>(Constants.ArbitraryAmrs, raw[Constants.ArbitraryAudiences]) || error;

            // make sure nothing in here is DISALLOWED
            if (!error)
            {
                var arbitraryClaims = raw[Constants.ArbitraryClaims];
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

            if (error)
            {
                context.Result.IsError = true;
                context.Result.Error = String.Join<string>(" | ", los); ;
            }
        }
    }


}