using System.Collections.Generic;
using GraphQL.Types;

namespace P7.GraphQLCore
{
    public interface IMutationFieldRecordRegistration
    {
        void AddGraphTypeFields(MutationCore mutationCore);
    }
}