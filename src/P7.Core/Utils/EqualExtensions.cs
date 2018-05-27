using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using P7.Core.Reflection;

namespace P7.Core.Utils
{

    public static class EqualExtensions
    {
        /// <summary>
        /// Compares two dictionaries for equality.
        /// </summary>
        /// <returns>
        /// True if the dictionaries have equal contents or are both null, otherwise false.
        /// </returns>
        public static bool DictionaryEqual<TKey, TValue>(
            this IDictionary<TKey, TValue> dict1, IDictionary<TKey, TValue> dict2,
            IEqualityComparer<TValue> equalityComparer = null)
        {
            if (dict1 == dict2)
                return true;

            if (dict1 == null | dict2 == null)
                return false;

            if (dict1.Count != dict2.Count)
                return false;

            if (equalityComparer == null)
                equalityComparer = EqualityComparer<TValue>.Default;

            return dict1.All(kvp =>
            {
                TValue value2;
                return dict2.TryGetValue(kvp.Key, out value2)
                       && equalityComparer.Equals(kvp.Value, value2);
            });
        }

        public static bool SafeEquals(this IDictionary<string, string> d1, IDictionary<string, string> d2)
        {
            bool isEqual = DictionaryEqual(d1, d2, StringComparer.Ordinal);
            return isEqual;
        }
        public static bool SafeEquals(this Dictionary<string, string> d1, Dictionary<string, string> d2)
        {
            bool isEqual = DictionaryEqual(d1, d2, StringComparer.Ordinal);
            return isEqual;
        }


        public static bool SafeEquals<T>(this T a, T b)
        {

            if (a == null && b == null)
                return true;
            if (a != null && b != null)
            {
                if (!a.IsGenericList())
                {
                    return a.Equals(b);
                }
            }

            return false;
        }
        public static bool SafeListEquals<T>(this List<T> a, List<T> b)
        {
            if (a == null && b == null)
                return true;
            if (a != null && b != null)
            {
                IEnumerable<T> difference = a.Except(b);
                var equals = !difference.Any();
                return equals;
            }
            return false;
        }
        public static bool SafeListEquals<T>(this IList<T> a, IList<T> b)
        {
            if (a == null && b == null)
                return true;
            if (a != null && b != null)
            {
                var difference = a.Except(b);
                var equals = !difference.Any();
                return equals;
            }
            return false;
        }
    }
}
