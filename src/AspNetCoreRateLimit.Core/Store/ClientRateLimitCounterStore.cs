using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreRateLimit.Services;
using Microsoft.Extensions.Options;

namespace AspNetCoreRateLimit.Core.Store
{
    public class ClientRateLimitCounterStore : IClientRateLimitCounterStore
    {
        private IRateLimitCounterStore _counterStore;
        private ClientRateLimitOptions _options;
        private RateLimitCore _core;
        private IClientRateLimitProcessor _processor;

        public ClientRateLimitCounterStore(
            IOptions<ClientRateLimitOptions> options,
            IClientRateLimitProcessor clientRateLimitProcessor,
            IRateLimitCounterStore counterStore,
            IClientPolicyStore policyStore)
        {
            _options = options.Value;
            _processor = clientRateLimitProcessor;
            _counterStore = counterStore;
            _core = new RateLimitCore(false, options.Value, _counterStore);
        }


        public async Task<IEnumerable<RateLimitCounterRecord>> GetRateLimitCounterRecordsAsync(ClientRequestIdentity identity)
        {
            var rules = _processor.GetMatchingRules(identity);
            List<RateLimitCounterRecord> rateLimitCounterRecords = new List<RateLimitCounterRecord>();
            foreach (var rule in rules)
            {
                RateLimitCounter? counter = _core.GetStoredRateLimitCounter(identity, rule);
                var record = new RateLimitCounterRecord
                {
                    RateLimitCounter = counter,
                    RateLimitRule = rule
                };
                rateLimitCounterRecords.Add(record);
            }
            return rateLimitCounterRecords;
        }
    }
}