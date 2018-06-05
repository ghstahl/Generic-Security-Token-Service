using System;

namespace GraphQLCore.ExtensionGrants.GraphQL
{
    public class ArbitraryResourceOwnerInputHandle : IComparable
    {
        /*
            client_id:arbitrary-resource-owner-client
            client_secret:secret
            scope:offline_access metal nitro aggregator_service.read_only
            arbitrary_claims:{ "role": ["application", "limited"],"query":["dashboard", "licensing"],"seatId":["1234abcd"]}
            subject:886bea3f-e025-4ab9-a811-e9b86f563668
            access_token_lifetime:3600
         */
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string arbitrary_claims { get; set; }
        public string scope { get; set; }
        public string subject { get; set; }
        public string access_token { get; set; }
        public int access_token_lifetime { get; set; }

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
            var other = obj as ArbitraryResourceOwnerInputHandle;
            if (other == null)
            {
                return false;
            }
            if (other.client_id != this.client_id || 
                other.client_secret != this.client_secret ||
                other.arbitrary_claims != this.arbitrary_claims ||
                other.scope != this.scope ||
                other.subject != this.subject ||
                other.access_token_lifetime != this.access_token_lifetime  
            )
            {
                return false;
            }
            return true;
        }
    }
}