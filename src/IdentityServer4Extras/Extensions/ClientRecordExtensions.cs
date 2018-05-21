using System.Collections.Generic;
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
                AllowedGrantTypes = self.AllowedGrantTypes,
                AllowOfflineAccess = self.AllowOfflineAccess,
                RefreshTokenUsage = (TokenUsage)self.RefreshTokenUsage,
                ClientSecrets = secrets,
                AllowedScopes = self.AllowedScopes,
                RequireClientSecret = self.RequireClientSecret,
                RequireRefreshClientSecret = self.RequireRefreshClientSecret,
                AccessTokenLifetime = self.AccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = self.AbsoluteRefreshTokenLifetime,
                ClientClaimsPrefix = self.ClientClaimsPrefix
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