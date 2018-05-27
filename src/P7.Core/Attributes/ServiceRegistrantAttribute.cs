using System;

namespace P7.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class , AllowMultiple = false, Inherited = true)]
    public class ServiceRegistrantAttribute: System.Attribute 
    {
        public Type[] Types { get; private set; }
        public ServiceRegistrantAttribute( params Type[] types)
        {
            Types = types;
        }
    }
}