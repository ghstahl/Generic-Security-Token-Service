using GraphQL.Types;

namespace GraphQLCore.ExtensionGrants.GraphQL
{
    public class ArbitraryNoSubjectResultType : ObjectGraphType<ArbitraryNoSubjectResult>
    {
        public ArbitraryNoSubjectResultType()
        {
            Name = "arbitraryNoSubjectResult";
            Field(x => x.access_token).Description("The access_token.");
            Field(x => x.expires_in).Description("Expired in seconds.");
            Field(x => x.token_type).Description("The type of token.");
        }
    }
}