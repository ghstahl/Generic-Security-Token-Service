using System.Diagnostics;

namespace GenericSecurityTokenService.Extensions
{
    public static class StringExtensions
    {
        [DebuggerStepThrough]
        public static string RemoveTrailingSlash(this string url)
        {
            if (url != null && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }
    }
}