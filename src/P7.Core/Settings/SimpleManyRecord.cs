using System.Collections.Generic;
using Newtonsoft.Json;

namespace P7.Core.Settings
{
    public static class SimpleManyExtensions
    {
        public static string ToString(this SimpleManyRecord smr)
        {
            string output = JsonConvert.SerializeObject(smr);
            return output;
        }
    }

    public class SimpleManyRecord
    {
        public string Filter { get; set; }
        public List<AreaNode> RouteTree { get; set; }

        public override string ToString()
        {
            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
            return output;
            /*
            string result = "";
            result += "Filter: " + Filter + "\n";
            result += "    RouteTree:[" + "\n";
            foreach (var area in RouteTree)
            {
                result += "      {\n";
                result += "        " + area;
                result += "      }\n";
            }
            result += "              ]" + "\n";
            return result;
            */
        }
    }
    public class GlobalPathRecord
    {
        public string Filter { get; set; }
        public List<string> Paths { get; set; }

        public override string ToString()
        {
            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
            return output;  
        }
    }
}