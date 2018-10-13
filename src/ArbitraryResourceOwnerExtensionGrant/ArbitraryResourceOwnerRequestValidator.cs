using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ArbitraryResourceOwnerExtensionGrant
{
    
    public class ArbitraryResourceOwnerRequestValidator
    {
 
        private readonly ILogger<ArbitraryResourceOwnerRequestValidator> _logger;

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
                                                                         "client_id",
                                                                         JwtClaimTypes.Subject,
                                                                         ClaimTypes.NameIdentifier
                                                                     });

        private static List<string> _oneMustExitsArguments;
        private static List<string> OneMustExitsArguments => _oneMustExitsArguments ??
                                                                  (_oneMustExitsArguments =
                                                                      new List<string>
                                                                      {
                                                                          "subject",
                                                                          "access_token"
                                                                      });

        public ArbitraryResourceOwnerRequestValidator(
            ILogger<ArbitraryResourceOwnerRequestValidator> logger)
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
 
            if (error)
            {
                context.Result.IsError = true;
                context.Result.Error = String.Join<string>(" | ", los); ;
            }
        }
    }
}