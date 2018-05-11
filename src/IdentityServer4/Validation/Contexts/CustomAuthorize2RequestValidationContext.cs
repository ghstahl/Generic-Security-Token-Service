namespace IdentityServer4.Validation
{
    /// <summary>
    /// Context class for custom token request validation
    /// </summary>
    public class CustomAuthorize2RequestValidationContext
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public Authorize2RequestValidationResult Result { get; set; }
    }
}