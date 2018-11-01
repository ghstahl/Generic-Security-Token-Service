using System.Collections.Generic;

namespace FunctionsCore.Modules
{
    public class MyContext
    {
        public string TraceIdentifier { get; set; }
        private Dictionary<string, string> _dictionary;

        public Dictionary<string, string> Dictionary
        {
            get { return _dictionary ?? (_dictionary = new Dictionary<string, string>()); }
        }
    }
}