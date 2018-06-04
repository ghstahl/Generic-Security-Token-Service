using System.Threading.Tasks;
using GraphQL.Types;

namespace P7.GraphQLCore
{
    public class QueryCore : ObjectGraphType<object>
    {
        public QueryCore()
        {
            
        }
        public QueryCore(IQueryFieldRecordRegistrationStore fieldStore)
        {
            Name = "query";
            fieldStore.AddGraphTypeFields(this);
        }
    }
}

