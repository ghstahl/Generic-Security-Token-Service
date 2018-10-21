using System;

namespace GraphQLCore.ExtensionGrants.GraphQL
{
    public class ArbitraryIdentityResult : IComparable
    {
        public string id_token { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }

        public int CompareTo(object obj)
        {
            if (Equals(obj))
                return 0;
            return -1;
        }
        public override bool Equals(object obj)
        {
            return ShallowEquals(obj);
        }
        public bool ShallowEquals(object obj)
        {
            var other = obj as ArbitraryIdentityResult;
            if (other == null)
            {
                return false;
            }
            if (other.id_token != this.id_token || 
                other.access_token != this.access_token ||
                other.token_type != this.token_type ||
                other.expires_in != this.expires_in ||
                other.refresh_token != this.refresh_token)
            {
                return false;
            }
            return true;
        }
    }
}