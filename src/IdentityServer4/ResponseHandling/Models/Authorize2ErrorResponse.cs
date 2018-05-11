using System.Collections.Generic;

namespace IdentityServer4.ResponseHandling
{
    /// <summary>
    /// Models a Authorize2 error response
    /// </summary>
    public class Authorize2ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; } = Authorize2Constants.Authorize2Errors.InvalidRequest;

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the custom entries.
        /// </summary>
        /// <value>
        /// The custom.
        /// </value>
        public Dictionary<string, object> Custom { get; set; } = new Dictionary<string, object>();
    }
}