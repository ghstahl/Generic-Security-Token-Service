using System;

namespace P7.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TaggerAttribute: System.Attribute 
    {
        public string Tag { get; set; }
        public TaggerAttribute(string tag)
        {
            Tag = tag;
        }
    }
}
