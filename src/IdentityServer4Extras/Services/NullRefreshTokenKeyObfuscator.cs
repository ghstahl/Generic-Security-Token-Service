using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4Extras.Services
{
    public class NullRefreshTokenKeyObfuscator : IRefreshTokenKeyObfuscator
    {
        public Task<string> ObfuscateAsync(string key)
        {
            return Task.FromResult(key);
        }

        public Task<string> UnObfuscateAsync(string key)
        {
            return Task.FromResult(key);
        }
    }
}
