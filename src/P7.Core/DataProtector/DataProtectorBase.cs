using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
 
namespace P7.Core.DataProtector
{
    public abstract class DataProtectorBase
    {
        private readonly IDataProtector _protector;
        private readonly ILogger _logger;

        public DataProtectorBase(IDataProtectionProvider provider, string purpose, ILogger logger)
        {
            _protector = provider.CreateProtector(purpose);
            _logger = logger;
        }
        public Task<string> ProtectAsync(string key)
        {
            try
            {
                var value = _protector.Protect(key);
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception writing protected key");
                throw;
            }
        }
        public Task<string> UnprotectAsync(string key)
        {
            try
            {
                var value = _protector.Unprotect(key);
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