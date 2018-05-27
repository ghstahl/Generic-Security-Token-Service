using System.Collections.Generic;
using System.Linq;

namespace P7.Core.Localization
{
    public static class ResourceApiExtensions
    {
        public static int GetSequenceHashCode<T>(this IEnumerable<T> sequence)
        {
            return sequence
                .Select(item => item.GetHashCode())
                .Aggregate((total, nextCode) => total ^ nextCode);
        }
    }
}