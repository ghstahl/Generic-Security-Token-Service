using System.Collections.Generic;
using System.Linq;

namespace P7.GraphQLCore
{
    public class MutationFieldRecordRegistrationStore :
        IMutationFieldRecordRegistrationStore
    {
        private List<IMutationFieldRecordRegistration> _fieldRecordRegistrations;

        public MutationFieldRecordRegistrationStore(
            IEnumerable<IMutationFieldRecordRegistration> fieldRecordRegistrations)
        {
            _fieldRecordRegistrations = fieldRecordRegistrations.ToList();
        }

        public void AddGraphTypeFields(MutationCore mutationCore)
        {
            foreach (var item in _fieldRecordRegistrations)
            {
                item.AddGraphTypeFields(mutationCore);
            }
        }
    }
}