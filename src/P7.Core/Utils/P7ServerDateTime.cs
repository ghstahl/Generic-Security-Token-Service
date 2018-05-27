using System;

namespace P7.Core.Utils
{
    public static class P7ServerDateTime
    {
        public static DateTime UtcNow => UtcNowFunc();
        public static Func<DateTime> UtcNowFunc = () => DateTime.UtcNow;
    }
}