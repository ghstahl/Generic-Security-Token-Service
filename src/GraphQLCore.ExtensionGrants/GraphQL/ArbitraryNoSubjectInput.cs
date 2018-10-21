using GraphQL.Types;

namespace GraphQLCore.ExtensionGrants.GraphQL
{
    public class ArbitraryNoSubjectInput : InputObjectGraphType
    {
        /*
        client_id:arbitrary-resource-owner-client
        client_secret:secret
        scope:offline_access metal nitro aggregator_service.read_only
        arbitrary_claims:{ "role": ["application", "limited"],"query":["dashboard", "licensing"],"seatId":["1234abcd"]}
        access_token_lifetime:3600
     */
        public ArbitraryNoSubjectInput()
        {
            Name = "arbitrary_no_subject";
            Field<NonNullGraphType<StringGraphType>>("client_id");
            Field<NonNullGraphType<StringGraphType>>("client_secret");
            Field<StringGraphType>("scope");
            Field<NonNullGraphType<StringGraphType>>("arbitrary_claims");
            Field<StringGraphType>("arbitrary_amrs");
            Field<StringGraphType>("arbitrary_audiences");
            Field<IntGraphType>("access_token_lifetime");
        }
    }
}