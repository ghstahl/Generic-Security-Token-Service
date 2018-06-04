using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;

namespace P7.GraphQLCore
{
    public class QueryFieldRecordRegistrationStore : IQueryFieldRecordRegistrationStore
    {
        private IEnumerable<IQueryFieldRecordRegistration> _fieldRecordRegistrations;
        public QueryFieldRecordRegistrationStore(IEnumerable<IQueryFieldRecordRegistration> fieldRecordRegistrations)
        {
            _fieldRecordRegistrations = fieldRecordRegistrations;
        }

        public void AddGraphTypeFields(QueryCore queryCore)
        {
            foreach (var item in _fieldRecordRegistrations)
            {
                item.AddGraphTypeFields(queryCore);
            }
        }
    }
}
