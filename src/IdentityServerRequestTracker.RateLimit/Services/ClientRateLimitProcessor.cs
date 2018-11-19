using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServerRequestTracker.RateLimit.Options;
using Microsoft.Extensions.Options;

namespace IdentityServerRequestTracker.RateLimit.Services
{
    internal class ClientRateLimitProcessor : IClientRateLimitProcessor
    {
        private readonly ClientRateLimitingOptions _options;
        private readonly IRateLimitCounterStore _counterStore;
        private readonly RateLimitCore _core;

        private static readonly object _processLocker = new object();

        public ClientRateLimitProcessor(
            IOptions<ClientRateLimitingOptions> options,
            IRateLimitCounterStore counterStore )
        {
            _options = options.Value;
            _counterStore = counterStore;
            _core = new RateLimitCore(false, options.Value, _counterStore);
        }

        public RateLimitClientsRule GetRateLimitClientsRule(ClientRequestIdentity identity)
        {
            var clientsRule = (from item in _options.Rules
                from clientId in item.ClientIds
                where clientId == identity.ClientId && item.Enabled
                select item).FirstOrDefault();
            if (clientsRule != null)
            {
                foreach (var item in clientsRule.Settings.RateLimitRules)
                {
                    //parse period text into time spans
                    item.PeriodTimespan = _core.ConvertToTimeSpan(item.Period);
                }
            }
            return clientsRule;
        }

        public RateLimitCounter ProcessRequest(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            return _core.ProcessRequest(requestIdentity, rule);
        }

        public RateLimitHeaders GetRateLimitHeaders(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            return _core.GetRateLimitHeaders(requestIdentity, rule);
        }

        public string RetryAfterFrom(DateTime timestamp, RateLimitRule rule)
        {
            return _core.RetryAfterFrom(timestamp, rule);
        }

    }
}