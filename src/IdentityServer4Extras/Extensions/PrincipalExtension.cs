using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using IdentityModel;

namespace IdentityServer4Extras.Extensions
{
    public static  class PrincipalExtension
    {
        /// <summary>
        /// Gets the named identifier.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string GetNamedIdentifier(this IPrincipal principal)
        {
            return principal.Identity.GetNamedIdentifier();
        }

        [DebuggerStepThrough]
        public static string GetSubjectOrNamedIdentifier(this IPrincipal principal)
        {
            return principal.Identity.GetSubjectOrNamedIdentifier();
        }

        /// <summary>
        /// Gets the named identifier.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">sub claim is missing</exception>
        [DebuggerStepThrough]
        public static string GetNamedIdentifier(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null) throw new InvalidOperationException("sub claim is missing");
            return claim.Value;
        }
        /// <summary>
        /// Gets the named identifier.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">sub claim is missing</exception>
        [DebuggerStepThrough]
        public static string GetSubjectOrNamedIdentifier(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id.FindFirst("sub");

            if (claim == null)
            {
                return identity.GetNamedIdentifier();
            }
            return claim.Value;
        }
    }
}
