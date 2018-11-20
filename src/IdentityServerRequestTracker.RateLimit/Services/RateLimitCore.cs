using System;
using System.Globalization;
using System.Linq;
using IdentityServerRequestTracker.RateLimit.Options;
using Microsoft.Extensions.Options;

namespace IdentityServerRequestTracker.RateLimit.Services
{
    public class RateLimitCore: IClientRateLimitProcessor
    {
        private readonly ClientRateLimitingOptions _options;
        private readonly IRateLimitCounterStore _counterStore;

        private static readonly object _processLocker = new object();

        public RateLimitCore( 
            IOptions<ClientRateLimitingOptions> options,
            IRateLimitCounterStore counterStore)
        {
            _options = options.Value;
            _counterStore = counterStore;
        }

        public string ComputeCounterKey(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            var key =  $"{requestIdentity.ClientId}_{rule.Period}";
            var idBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] hashBytes;
            using (var algorithm = System.Security.Cryptography.SHA1.Create())
            {
                hashBytes = algorithm.ComputeHash(idBytes);
            }
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
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
                    item.PeriodTimespan = ConvertToTimeSpan(item.Period);
                }
            }
            return clientsRule;
        }
        public RateLimitCounter GetCurrentRateLimitCounter(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            var counter = new RateLimitCounter
            {
                Timestamp = DateTime.UtcNow,
                TotalRequests = 0
            };

            var counterId = ComputeCounterKey(requestIdentity, rule);

            // serial reads and writes
            lock (_processLocker)
            {
                var entry = _counterStore.Get(counterId);
                if (entry.HasValue)
                {
                    // deep copy
                    counter = new RateLimitCounter
                    {
                        Timestamp = entry.Value.Timestamp,
                        TotalRequests = entry.Value.TotalRequests
                    };
                }
            }
            return counter;
        }
        public RateLimitCounter ProcessRequest(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            var counter = new RateLimitCounter
            {
                Timestamp = DateTime.UtcNow,
                TotalRequests = 1
            };

            var counterId = ComputeCounterKey(requestIdentity, rule);

            // serial reads and writes
            lock (_processLocker)
            {
                var entry = _counterStore.Get(counterId);
                if (entry.HasValue)
                {
                    // entry has not expired
                    if (entry.Value.Timestamp + rule.PeriodTimespan.Value >= DateTime.UtcNow)
                    {
                        // increment request count
                        var totalRequests = entry.Value.TotalRequests + 1;

                        // deep copy
                        counter = new RateLimitCounter
                        {
                            Timestamp = entry.Value.Timestamp,
                            TotalRequests = totalRequests
                        };
                    }
                }

                // stores: id (string) - timestamp (datetime) - total_requests (long)
                _counterStore.Set(counterId, counter, rule.PeriodTimespan.Value);
            }

            return counter;
        }
        public RateLimitCounter? GetStoredRateLimitCounter(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            var counterId = ComputeCounterKey(requestIdentity, rule);

            // serial reads and writes
            lock (_processLocker)
            {
                var entry = _counterStore.Get(counterId);
                if (entry.HasValue)
                {
                    // entry has not expired
                    if (entry.Value.Timestamp + rule.PeriodTimespan.Value >= DateTime.UtcNow)
                    {

                        // deep copy
                        var counter = new RateLimitCounter
                        {
                            Timestamp = entry.Value.Timestamp,
                            TotalRequests = entry.Value.TotalRequests
                        };
                        return counter;
                    }
                }
            }
            return null;
        }
        public RateLimitHeaders GetRateLimitHeaders(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            var headers = new RateLimitHeaders();
            var counterId = ComputeCounterKey(requestIdentity, rule);
            var entry = _counterStore.Get(counterId);
            if (entry.HasValue)
            {
                headers.Reset = (entry.Value.Timestamp + ConvertToTimeSpan(rule.Period)).ToUniversalTime()
                    .ToString("o", DateTimeFormatInfo.InvariantInfo);
                headers.Limit = rule.Period;
                headers.Remaining = (rule.Limit - entry.Value.TotalRequests).ToString();
            }
            else
            {
                headers.Reset = (DateTime.UtcNow + ConvertToTimeSpan(rule.Period)).ToUniversalTime()
                    .ToString("o", DateTimeFormatInfo.InvariantInfo);
                headers.Limit = rule.Period;
                headers.Remaining = rule.Limit.ToString();
            }

            return headers;
        }

        public string RetryAfterFrom(DateTime timestamp, RateLimitRule rule)
        {
            var secondsPast = Convert.ToInt32((DateTime.UtcNow - timestamp).TotalSeconds);
            var retryAfter = Convert.ToInt32(rule.PeriodTimespan.Value.TotalSeconds);
            retryAfter = retryAfter > 1 ? retryAfter - secondsPast : 1;
            return retryAfter.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public TimeSpan ConvertToTimeSpan(string timeSpan)
        {
            var l = timeSpan.Length - 1;
            var value = timeSpan.Substring(0, l);
            var type = timeSpan.Substring(l, 1);

            switch (type)
            {
                case "d": return TimeSpan.FromDays(double.Parse(value));
                case "h": return TimeSpan.FromHours(double.Parse(value));
                case "m": return TimeSpan.FromMinutes(double.Parse(value));
                case "s": return TimeSpan.FromSeconds(double.Parse(value));
                default: throw new FormatException($"{timeSpan} can't be converted to TimeSpan, unknown type {type}");
            }
        }
    }
}