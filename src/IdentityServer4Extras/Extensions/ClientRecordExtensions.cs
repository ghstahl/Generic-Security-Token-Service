using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4Extras.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServer4Extras.Extensions
{
    public static class ClientRecordExtensions
    {
        
        public static List<Client> LoadClientsFromSettings(this IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("clients");
            var clientRecords = new Dictionary<string, ClientRecord>();
            section.Bind(clientRecords);
            foreach (var clientRecord in clientRecords)
            {
                clientRecord.Value.ClientId = clientRecord.Key;
            }
            var clients = clientRecords.ToClients();
            return clients;
        }

        public static List<IdentityResource> LoadIdentityResourcesFromSettings(this IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("identityResources");
            var identityResources = new List<string>();
            section.Bind(identityResources);
            List<IdentityResource> result = new List<IdentityResource>();
            foreach (var identityResource in identityResources)
            {
                switch (identityResource)
                {
                    case IdentityServerConstants.StandardScopes.OpenId:
                        result.Add(new IdentityResources.OpenId());
                        break;
                    case IdentityServerConstants.StandardScopes.Profile:
                        result.Add(new IdentityResources.Profile());
                        break;
                    case IdentityServerConstants.StandardScopes.Email:
                        result.Add(new IdentityResources.Email());
                        break;
                    case IdentityServerConstants.StandardScopes.Phone:
                        result.Add(new IdentityResources.Phone());
                        break;
                    case IdentityServerConstants.StandardScopes.Address:
                        result.Add(new IdentityResources.Address());
                        break;
                }
            }

            return result;
        }

        public static Client ToClient(this ClientRecord self)
        {
            List<Secret> secrets = new List<Secret>();
            foreach (var secret in self.Secrets)
            {
                secrets.Add(new Secret(secret.Sha256()));
            }

            return new ClientExtra()
            {
                ClientId = self.ClientId,

                AbsoluteRefreshTokenLifetime = self.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = self.AccessTokenLifetime,
                AllowedGrantTypes = self.AllowedGrantTypes,
                AllowedScopes = self.AllowedScopes,
                AllowOfflineAccess = self.AllowOfflineAccess,
                AccessTokenType = (AccessTokenType)self.AccessTokenType,
                ClientClaimsPrefix = self.ClientClaimsPrefix,
                ClientSecrets = secrets,
                Enabled = self.Enabled,
                FrontChannelLogoutSessionRequired = self.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = self.FrontChannelLogoutUri,
                PostLogoutRedirectUris = self.PostLogoutRedirectUris,
                RedirectUris = self.RedirectUris,
                RefreshTokenUsage = (TokenUsage)self.RefreshTokenUsage,
                RequireClientSecret = self.RequireClientSecret,
                RequireConsent = self.RequireConsent,
                RequireRefreshClientSecret = self.RequireRefreshClientSecret,
                SlidingRefreshTokenLifetime = self.SlidingRefreshTokenLifetime,
                Namespace = self.Namespace
         
            };
        }

        public static List<Client> ToClients(this Dictionary<string, ClientRecord> self)
        {
            List<Client> result = new List<Client>();
            foreach (var clientRecord in self)
            {
                clientRecord.Value.ClientId = clientRecord.Key;
                var client = clientRecord.Value.ToClient();
                result.Add(client);
            }

            return result;
        }
    }
}