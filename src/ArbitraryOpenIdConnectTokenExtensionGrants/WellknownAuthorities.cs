using System.Collections.Generic;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    internal static class WellknownAuthorities
    {
        private static Dictionary<string, AuthorityConfig> _authoritiesMap { get; set; }

        public static Dictionary<string, AuthorityConfig> AuthoritiesMap
        {
            get
            {
                if (_authoritiesMap == null)
                {
                    _authoritiesMap = new Dictionary<string, AuthorityConfig>()
                    {
                        {
                            "wellknown://norton-int", new AuthorityConfig()
                            {
                                Name = "wellknown://norton-int",
                                Authority = "https://login-int.norton.com/sso/oidc1/token",
                                AdditionalEndpointBaseAddresses = new List<string>()
                                {
                                    "https://login-int.norton.com/sso/idp/OIDC",
                                    "https://login-int.norton.com/sso/oidc1"
                                }
                            }
                        },
                        {
                            "wellknown://norton", new AuthorityConfig()
                            {
                                Name = "wellknown://norton",
                                Authority = "https://login.norton.com/sso/oidc1/token",
                                AdditionalEndpointBaseAddresses = new List<string>()
                                {
                                    "https://login.norton.com/sso/idp/OIDC",
                                    "https://login.norton.com/sso/oidc1"
                                }
                            }
                        },
                        {
                            "wellknown://google", new AuthorityConfig()
                            {
                                Name = "wellknown://google",
                                Authority = "https://accounts.google.com",
                                AdditionalEndpointBaseAddresses = new List<string>()
                            }
                        }
                    };
                }

                return _authoritiesMap;
            }
        }
    }
}