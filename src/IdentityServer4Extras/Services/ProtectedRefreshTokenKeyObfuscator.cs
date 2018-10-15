using System;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using P7.Core.Utils;

namespace IdentityServer4Extras.Services
{
    public class ProtectedRefreshTokenKeyObfuscator : IRefreshTokenKeyObfuscator
    {
        private const string Purpose = "IdentityServer4Extras.Services.ProtectedRefreshTokenKeyObfuscator";

        private readonly IDataProtector _protector;
        private readonly ILogger _logger;

        public ProtectedRefreshTokenKeyObfuscator(IDataProtectionProvider provider, ILogger<ProtectedRefreshTokenKeyObfuscator> logger)
        {
            _protector = provider.CreateProtector(Purpose);
            _logger = logger;
        }

        public Task<string> ObfuscateAsync(string key)
        {
            Guard.ArgumentNotNull(nameof(key), key);
            try
            {
                var bytes = Encoding.UTF8.GetBytes(key);
                bytes = _protector.Protect(bytes);
                var value = Base64Url.Encode(bytes);
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception writing protected key");
                throw;
            }
        }

        public Task<string> UnObfuscateAsync(string key)
        {
            Guard.ArgumentNotNull(nameof(key), key);
            try
            {
                var bytes = Base64Url.Decode(key);
                bytes = _protector.Unprotect(bytes);
                var value = Encoding.UTF8.GetString(bytes);
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception reading protected key");
                throw;
            }
        }
    }
}