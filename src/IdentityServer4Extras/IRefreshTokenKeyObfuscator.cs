using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4Extras
{
    public interface IRefreshTokenKeyObfuscator
    {
        Task<string> ObfuscateAsync(string key);
        Task<string> UnObfuscateAsync(string key);
    }
}
