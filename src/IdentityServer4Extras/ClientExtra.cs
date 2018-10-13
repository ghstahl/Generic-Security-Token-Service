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


        //
        // Summary:
        //     Watermark this automatically put in to give some hint to the end user of the token as who did it.
        public string Watermark { get; set; }
    }
}
