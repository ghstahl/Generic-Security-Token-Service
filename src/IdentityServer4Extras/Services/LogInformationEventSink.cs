using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Services
{
    public class LogInformationEventSink : IAggregateEventSink
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogInformationEventSink"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public LogInformationEventSink(ILogger<DefaultEventService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public virtual Task PersistAsync(Event evt)
        {
            if (evt == null)
                throw new ArgumentNullException(nameof(evt));

            _logger.LogInformation("{@event}", evt);

            return Task.CompletedTask;
        }
    }
}