using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Validation;

namespace IdentityServer4Extras.Validation
{
    public class TokenRevocationRequestValidationResultExtra : TokenRevocationRequestValidationResult
    {

        /// <summary>
        /// Gets or sets whether all subjects that this token refers to also gets revoked.
        /// </summary>
        /// <value>
        /// Revocation of all subjects directive.
        /// </value>
        public bool RevokeAllAssociatedSubjects { get; set; }
    }
}
