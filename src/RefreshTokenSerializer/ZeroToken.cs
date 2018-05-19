using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Models;
using ZeroFormatter;

namespace RefreshTokenSerializer
{
    /// <summary>
    /// Models a token.
    /// </summary>
    [ZeroFormattable]
    public class ZeroToken : IEquatable<ZeroToken>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        public ZeroToken()
        {
        }

        /// <summary>
        /// Gets or sets the audiences.
        /// </summary>
        /// <value>
        /// The audiences.
        /// </value>
        [Index(0)]
        public virtual List<string> Audiences { get; set; }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>
        /// The issuer.
        /// </value>
        [Index(1)]
        public virtual string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        [Index(2)]
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>
        /// The lifetime.
        /// </value>
        [Index(3)]
        public virtual int Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Index(4)]
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the ID of the client.
        /// </summary>
        /// <value>
        /// The ID of the client.
        /// </value>
        [Index(5)]
        public virtual string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the type of access token of the client
        /// </summary>
        /// <value>
        /// The access token type specified by the client.
        /// </value>
        [Index(6)]
        public virtual AccessTokenType AccessTokenType { get; set; }

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        /// <value>
        /// The claims.
        /// </value>
        [Index(7)]
        public virtual List<ZeroClaim> Claims { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [Index(8)]
        public virtual int Version { get; set; }

        public bool Equals(ZeroToken other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other)) return true;

            bool audEqual = Audiences.Count == other.Audiences.Count &&
                            Audiences.All(other.Audiences.Contains);
            bool claimsEqual = Claims.Count == other.Claims.Count &&
                               Claims.All(other.Claims.Contains);
            bool result = audEqual &&
                          string.Equals(Issuer, other.Issuer) &&
                          CreationTime.Equals(other.CreationTime) &&
                          Lifetime == other.Lifetime &&
                          string.Equals(Type, other.Type) &&
                          string.Equals(ClientId, other.ClientId) &&
                          AccessTokenType == other.AccessTokenType &&
                          claimsEqual &&
                          Version == other.Version;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj)) return true;
             
            bool result =  Equals((ZeroToken) obj);
            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Audiences != null ? Audiences.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Issuer != null ? Issuer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CreationTime.GetHashCode();
                hashCode = (hashCode * 397) ^ Lifetime;
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ClientId != null ? ClientId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) AccessTokenType;
                hashCode = (hashCode * 397) ^ (Claims != null ? Claims.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Version;
                return hashCode;
            }
        }
    }
}