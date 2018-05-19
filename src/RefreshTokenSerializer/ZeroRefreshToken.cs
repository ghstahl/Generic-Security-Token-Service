using System;
using System.Linq;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using ZeroFormatter;

namespace RefreshTokenSerializer
{
    /// <summary>
    /// Models a refresh token.
    /// </summary>
    [ZeroFormattable]
    public class ZeroRefreshToken: IEquatable<ZeroRefreshToken>
    {
        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        [Index(0)]
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the life time.
        /// </summary>
        /// <value>
        /// The life time.
        /// </value>
        [Index(1)]
        public virtual int Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [Index(2)]
        public virtual ZeroToken AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [Index(3)]
        public virtual int Version { get; set; }

        public bool Equals(ZeroRefreshToken other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other)) return true;
            bool result = CreationTime.Equals(other.CreationTime) &&
                          Lifetime == other.Lifetime &&
                          Equals(AccessToken, other.AccessToken) &&
                          Version == other.Version;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj)) return true;
 
            return Equals((ZeroRefreshToken) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CreationTime.GetHashCode();
                hashCode = (hashCode * 397) ^ Lifetime;
                hashCode = (hashCode * 397) ^ (AccessToken != null ? AccessToken.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Version;
                return hashCode;
            }
        }
    }
}
