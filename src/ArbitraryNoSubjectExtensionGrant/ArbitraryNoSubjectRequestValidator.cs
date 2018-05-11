﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

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
        public ArbitraryNoSubjectRequestValidator(
            ILogger<ArbitraryNoSubjectRequestValidator> logger)
        {
            _logger = logger;
        }
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var raw = context.Result.ValidatedRequest.Raw;
            var rr = raw.AllKeys.ToDictionary(k => k, k => raw[(string)k]);
            var error = false;
            var los = new List<string>();

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