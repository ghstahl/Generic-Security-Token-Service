using GraphQL.Types;
using GraphQLCore.ExtensionGrants.Models;

namespace GraphQLCore.ExtensionGrants.GraphQL
{

    public class IdentityModelType : ObjectGraphType<GraphQLCore.ExtensionGrants.Models.IdentityModel>
    {
        public IdentityModelType()
        {
            Name = "identity";
            Field<ListGraphType<ClaimHandleType>>("claims", "The Claims of the identity");
        }
    }
}