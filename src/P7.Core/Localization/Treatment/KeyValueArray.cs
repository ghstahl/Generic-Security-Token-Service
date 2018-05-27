using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace P7.Core.Localization.Treatment
{
    public class KeyValueArray: ILocalizedStringResultTreatment
    {
        public object Process(IEnumerable<LocalizedString> resourceSet)
        {
            var result = (
                    from  entry in resourceSet
                    select new StringResourceSet { Key = entry.Name, Value = entry.Value })
                .ToList();

            return result;
        }

        public string Key => "kva";
    }
}