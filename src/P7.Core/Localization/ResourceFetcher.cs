using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using P7.Core.Localization.Treatment;
using P7.Core.Reflection;

namespace P7.Core.Localization
{
    public class ResourceFetcher: IResourceFetcher
    {
        private IStringLocalizerFactory _localizerFactory;
        private IMemoryCache _cache;
        private ITreatmentMap _treatmentMap;
        public  ResourceFetcher(
            IStringLocalizerFactory localizerFactory,
            IMemoryCache cache,
            ITreatmentMap treatmentMap)
        {
            _localizerFactory = localizerFactory;
            _cache = cache;
            _treatmentMap = treatmentMap;
        }
 
        private object GetResourceSet(string id, string treatment, CultureInfo cultureInfo)
        {
            try
            {
                var typeId = TypeHelper<Type>.GetTypeByFullName(id);
                if (typeId != null)
                {
                    if (string.IsNullOrEmpty(treatment))
                    {
                        treatment = "kvo";
                    }
                    var treatmentObject = _treatmentMap.GetTreatment(treatment);
                    if (treatmentObject == null)
                    {
                        treatment = "kvo";
                        treatmentObject = _treatmentMap.GetTreatment(treatment);
                    }

                    var localizer = _localizerFactory.Create(typeId);

                    var resourceSet = localizer.WithCulture(cultureInfo).GetAllStrings(true);
                    var result = treatmentObject.Process(resourceSet);
                    return result;
                }
            }
            catch (Exception e)
            {
                return "";
            }
            return "";
        }

        public object GetResourceSet(ResourceQueryHandle input)
        {
            CultureInfo currentCulture = new CultureInfo(input.Culture);
            var key = new List<object> { input.Culture, input.Id, input.Treatment }
                .AsReadOnly().GetSequenceHashCode();
            var newValue = new Lazy<object>(() =>
            {
                return GetResourceSet(input.Id, input.Treatment, currentCulture);
            });
            var value = _cache.GetOrCreate(key.ToString(CultureInfo.InvariantCulture), entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(100);
                return newValue;
            });

            var result = value != null ? value.Value : newValue.Value;
            return result;
        }

        public Task<object> GetResourceSetAsync(ResourceQueryHandle input)
        {
            var result = Task.Run(() => GetResourceSet(input));
            return result;
        }
    }
}