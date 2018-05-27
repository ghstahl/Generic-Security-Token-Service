using System.Collections.Generic;

namespace P7.Core.Settings
{
    public class FiltersConfig
    {
        public const string WellKnown_SectionName = "Filters";
        public SimpleManyConfig SimpleMany { get; set; }
        public GlobalPathConfig GlobalPath { get; set; }
        public MiddleWareConfig MiddleWare { get; set; }
    }

    public class MiddleWareConfig
    {
        public ProtectLocalOnlyConfig ProtectLocalOnly { get; set; }
    }

    public class ProtectLocalOnlyConfig
    {
        public List<string> Paths { get; set; }
    }
}
