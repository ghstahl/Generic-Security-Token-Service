using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;
using GraphQLCore.ExtensionGrants.Models;

namespace GraphQLCore.ExtensionGrants.GraphQL
{
    public class ClaimHandleType : ObjectGraphType<ClaimHandle>
    {
        public ClaimHandleType()
        {
            Name = "claim";

            Field(x => x.Name).Description("name of claim.");
            Field(x => x.Value).Description("value of claim.");
        }
    }
}
