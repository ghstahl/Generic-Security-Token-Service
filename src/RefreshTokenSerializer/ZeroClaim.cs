using System;
using ZeroFormatter;

namespace RefreshTokenSerializer
{
    [ZeroFormattable]
    public class ZeroClaim: IEquatable<ZeroClaim>
    {
        public ZeroClaim() { }
        public ZeroClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }
        [Index(0)]
        public virtual string Type { get; set; }
        [Index(1)]
        public virtual string Value { get; set; }

        public bool Equals(ZeroClaim other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other)) return true;
            bool result = string.Equals(Type, other.Type) && 
                          string.Equals(Value, other.Value);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
 
            return Equals((ZeroClaim) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }
    }
}