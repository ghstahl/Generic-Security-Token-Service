using System;
using System.Diagnostics;

namespace P7.Core.Utils
{
    public static class DateTimeExtensions
    {
        [DebuggerStepThrough]
        public static DateTime ToSecondResolution(this DateTime dateTime)
        {
            DateTime result = new DateTime(dateTime.Year,dateTime.Month,dateTime.Day,dateTime.Hour,dateTime.Minute,dateTime.Second,dateTime.Kind);
            return result;
        }

        [DebuggerStepThrough]
        public static bool HasExceeded(this DateTime creationTime, int seconds)
        {
            return (P7ServerDateTime.UtcNow > creationTime.AddSeconds(seconds));
        }

        [DebuggerStepThrough]
        public static int GetLifetimeInSeconds(this DateTime creationTime)
        {
            return ((int)(P7ServerDateTime.UtcNow - creationTime).TotalSeconds);
        }

        [DebuggerStepThrough]
        public static bool HasExpired(this DateTime? expirationTime)
        {
            if (expirationTime.HasValue &&
                expirationTime.Value.HasExpired())
            {
                return true;
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool HasExpired(this DateTime expirationTime)
        {
            if (expirationTime < P7ServerDateTime.UtcNow)
            {
                return true;
            }

            return false;
        }
    }
}