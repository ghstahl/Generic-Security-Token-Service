using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

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
                los.AddRange(OneMustExitsArguments.Select(item => $"[on or the other] {item} is missing!"));
            }
            var result = RequiredArbitraryArguments.Except(rr.Keys);
            if (result.Any())
            {
                error = true;
                los.AddRange(result.Select(item => $"{item} is missing!"));

            }
            if (error)
            {
                context.Result.IsError = true;
                context.Result.Error = String.Join<string>(" | ", los); ;
            }
        }
    }
}