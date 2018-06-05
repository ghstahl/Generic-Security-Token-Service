using GraphQL.Types;

namespace GraphQLCore.ExtensionGrants.GraphQL
{
    public class ArbitraryResourceOwnerResultType : ObjectGraphType<ArbitraryResourceOwnerResult>
    {
        public ArbitraryResourceOwnerResultType()
        {
            Name = "arbitraryResourceOwnerResult";
            Field(x => x.access_token).Description("The access_token.");
            Field(x => x.expires_in).Description("Expired in seconds.");
            Field(x => x.token_type).Description("The type of token.");
            Field(x => x.refresh_token).Description("The refresh_token.");
        }
    }
}