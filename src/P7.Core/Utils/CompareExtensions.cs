using System.Text;

namespace P7.Core.Utils
{
    public static class CompareExtensions
    {
        public static bool IsEqual(this object a, object b)
        {
            var bothNull = a == null && b == null;
            var bothNotNull = a != null && b != null;
            if (bothNotNull)
            {
                if (!a.Equals(b))
                    return false;
            }
            else if (!bothNull)
            {
                return false;
            }
            return true;
        }
    }
}
