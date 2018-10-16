using System;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using P7.Core.DataProtector;
using P7.Core.Utils;

namespace IdentityServer4Extras.Services
{

    public class ProtectedRefreshTokenKeyObfuscator : DataProtectorBase, IRefreshTokenKeyObfuscator
    {
        private const string Purpose = "IdentityServer4Extras.Services.ProtectedRefreshTokenKeyObfuscator";

        private readonly IDataProtector _protector;
        private readonly ILogger _logger;

        public ProtectedRefreshTokenKeyObfuscator(
            IDataProtectionProvider provider, ILogger<ProtectedRefreshTokenKeyObfuscator> logger):
            base(provider, Purpose,logger )
        {
            _protector = provider.CreateProtector(Purpose);
            _logger = logger;
        }

        public Task<string> ObfuscateAsync(string key)
        {
            return Task.FromResult(ProtectAsync(key).GetAwaiter().GetResult());
        }

        public Task<string> UnObfuscateAsync(string key)
        {
            return Task.FromResult(UnprotectAsync(key).GetAwaiter().GetResult());
        }
    }
}