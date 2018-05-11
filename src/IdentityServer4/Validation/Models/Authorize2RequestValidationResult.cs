using System.Collections.Generic;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validation result for token requests
    /// </summary>
    public class Authorize2RequestValidationResult : ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRequestValidationResult"/> class.
        /// </summary>
        /// <param name="validatedRequest">The validated request.</param>
        /// <param name="customResponse">The custom response.</param>
        public Authorize2RequestValidationResult(ValidatedTokenRequest validatedRequest, Dictionary<string, object> customResponse = null)
        {
            IsError = false;

            ValidatedRequest = validatedRequest;
            CustomResponse = customResponse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRequestValidationResult"/> class.
        /// </summary>
        /// <param name="validatedRequest">The validated request.</param>
        /// <param name="error">The error.</param>
        /// <param name="errorDescription">The error description.</param>
        /// <param name="customResponse">The custom response.</param>
        public Authorize2RequestValidationResult(ValidatedTokenRequest validatedRequest, string error, string errorDescription = null, Dictionary<string, object> customResponse = null)
        {
            IsError = true;

            Error = error;
            ErrorDescription = errorDescription;
            ValidatedRequest = validatedRequest;
            CustomResponse = customResponse;
        }

        /// <summary>
        /// Gets the validated request.
        /// </summary>
        /// <value>
        /// The validated request.
        /// </value>
        public ValidatedTokenRequest ValidatedRequest { get; }

        /// <summary>
        /// Gets or sets the custom response.
        /// </summary>
        /// <value>
        /// The custom response.
        /// </value>
        public Dictionary<string, object> CustomResponse { get; set; }
    }
}