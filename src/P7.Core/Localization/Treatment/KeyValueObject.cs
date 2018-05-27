using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace P7.Core.Localization.Treatment
{
    public class KeyValueObject: ILocalizedStringResultTreatment
    {
        public object Process(IEnumerable<LocalizedString> resourceSet)
        {
            var map = new Dictionary<string,string>();
            foreach (var rs in resourceSet)
            {
                map[rs.Name] = rs.Value;
            }
            return map;
        }
        public string Key => "kvo";
    }
}
