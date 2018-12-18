using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Services
{
    public class AggretatedEventSink : IEventSink
    {
        private ILogger<AggretatedEventSink> _logger;
        private IEnumerable<IAggregateEventSink> _eventSinks;

        public AggretatedEventSink(
            ILogger<AggretatedEventSink> logger,
            IEnumerable<IAggregateEventSink> eventSinks)
        {
            _logger = logger;
            _eventSinks = eventSinks;
        }
        public async Task PersistAsync(Event evt)
        {
            foreach (var eventSink in _eventSinks)
            {
                await eventSink.PersistAsync(evt);
            }
        }
    }
}
