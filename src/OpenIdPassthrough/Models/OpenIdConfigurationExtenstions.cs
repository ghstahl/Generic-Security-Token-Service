using Newtonsoft.Json;

namespace OpenIdPassthrough.Models
{
    public static class OpenIdConfigurationExtenstions
    {
        public static OpenIdConfiguration FromJson(this OpenIdConfiguration self, string json)
        {
            return JsonConvert.DeserializeObject<OpenIdConfiguration>(json, Converter.Settings);
        }
        public static string ToJson(this OpenIdConfiguration self, string json)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }
}