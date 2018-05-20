using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Models;

namespace IdentityServer4Extras
{
    public class ClientExtra : Client
    {
        private bool? _requireRefreshClientSecret;

        //
        // Summary:
        //     If set to false, no client secret is needed to refresh tokens at the token endpoint
        //     (defaults to RequireClientSecret)
        public bool RequireRefreshClientSecret
        {
            get
            {
                if (_requireRefreshClientSecret == null || RequireClientSecret == false)
                    return RequireClientSecret;
                return (bool)_requireRefreshClientSecret;
            }
            set => _requireRefreshClientSecret = value;
        }
    }
}
