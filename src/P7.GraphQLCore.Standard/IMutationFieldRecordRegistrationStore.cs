using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

namespace P7.GraphQLCore
{
    public interface IMutationFieldRecordRegistrationStore
    {
        void AddGraphTypeFields(MutationCore mutationCore);
    }
}

