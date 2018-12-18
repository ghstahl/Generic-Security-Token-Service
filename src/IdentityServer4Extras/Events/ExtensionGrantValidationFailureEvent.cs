using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Events;

namespace IdentityServer4Extras.Events
{
    /// <summary>
    /// Event for failed client authentication
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class ExtensionGrantValidationFailureEvent : Event
    {
        /// <summary>
        /// Gets or sets the grant_type.
        /// </summary>
        /// <value>
        /// The grant_type.
        /// </value>
        public string GrantType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServer4.Events.ClientAuthenticationFailureEvent"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="message">The message.</param>
        public ExtensionGrantValidationFailureEvent(string clientId, string grantType, string message)
            : base(EventCategories.Authentication,
                "Extension Grant Validation Failure",
                EventTypes.Failure,
                EventIds.ClientAuthenticationFailure,
                message)
        {
            GrantType = grantType;
            ClientId = clientId;
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }
    }
}
