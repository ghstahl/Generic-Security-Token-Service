using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Console_ExtensionGrantClient
{
    public class Program
    {
        private static IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        public static async Task MainAsync(string[] args)
        {
            var issuer = Environment.GetEnvironmentVariable("ISSUER");
            var documentRetriever = new HttpDocumentRetriever { RequireHttps = issuer.StartsWith("https://") };
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{issuer}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
            var config = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
           
            var audience = Environment.GetEnvironmentVariable("AUDIENCE");

        }
    }
   
}
