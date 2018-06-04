using System;
using GraphQL;
using GraphQL.Types;
using P7.GraphQLCore;

namespace AuthHandler.GraphQL
{
    public class PlaceHolderMutation : IMutationFieldRecordRegistration
    {
        public void AddGraphTypeFields(MutationCore mutationCore)
        {
            mutationCore.FieldAsync<BooleanGraphType>(name: "__placeHolder",
                description: "This is here so we have at least one mutation",
                resolve: async context => true,
                deprecationReason: null
            );
        }
    }
}