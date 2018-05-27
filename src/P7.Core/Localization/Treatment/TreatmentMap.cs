using System.Collections.Generic;

namespace P7.Core.Localization.Treatment
{
    public class TreatmentMap: ITreatmentMap
    {
        public TreatmentMap(IEnumerable<ILocalizedStringResultTreatment> treatments)
        {
            foreach (var treatment in treatments)
            {
                TheMap.Add(treatment.Key,treatment);
            }
        }
        
        private Dictionary<string, ILocalizedStringResultTreatment> _map;

        private Dictionary<string, ILocalizedStringResultTreatment> TheMap
        {
            get { return _map ?? (_map = new Dictionary<string, ILocalizedStringResultTreatment>()); }
        }

        public ILocalizedStringResultTreatment GetTreatment(string key)
        {
            ILocalizedStringResultTreatment value = null;
            if (TheMap.TryGetValue(key, out value))
                return value;
            return null;
        }
    }
}