using System;
using System.Collections.Generic;
using System.Text;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    public class AuthorityConfig
    {
        public string Name { get; set; }
        public string Authority { get; set; }
        public List<string> AdditionalEndpointBaseAddresses { get; set; }
    }
}
