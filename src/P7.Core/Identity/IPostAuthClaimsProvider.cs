using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace P7.Core.Identity
{
    public interface IPostAuthClaimsProvider
    {
        Task<List<Claim>> FetchClaims(ClaimsPrincipal principal);
    }
}
