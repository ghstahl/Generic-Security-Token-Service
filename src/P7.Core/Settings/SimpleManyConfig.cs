using System.Collections.Generic;

namespace P7.Core.Settings
{
    public class SimpleManyConfig
    {
        public List<SimpleManyRecord> OptOut { get; set; }
        public List<SimpleManyRecord> OptIn { get; set; }
    }
}