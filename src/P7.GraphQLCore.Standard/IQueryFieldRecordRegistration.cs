using System.Collections;
using System.Collections.Generic;
using GraphQL.Language.AST;

namespace P7.GraphQLCore
{
    public interface IQueryFieldRecordRegistration
    {
        void AddGraphTypeFields(QueryCore queryCore);
    }
}