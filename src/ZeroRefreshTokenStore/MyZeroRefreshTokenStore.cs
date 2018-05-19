using System;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using RefreshTokenSerializer;
using ZeroFormatter;

namespace ZeroRefreshTokenStore
{
    public class MyZeroRefreshTokenStore :  IRefreshTokenStore
    {
        public async Task<string> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            var zeroRefreshToken = refreshToken.ToZeroRefreshToken();
            var byteArray = ZeroFormatterSerializer.Serialize<ZeroRefreshToken>(zeroRefreshToken);
            string tokenHandle = Convert.ToBase64String(byteArray);
            return tokenHandle;
        }

        public async Task UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            try
            {
                byte[] byteArray = Convert.FromBase64String(refreshTokenHandle);
                var zeroRefreshToken = ZeroFormatterSerializer.Deserialize<ZeroRefreshToken>(byteArray);
                var refreshToken = zeroRefreshToken.ToRefreshToken();
                return refreshToken;
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public async Task RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            
        }

        public async Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
             
        }
    }
}
