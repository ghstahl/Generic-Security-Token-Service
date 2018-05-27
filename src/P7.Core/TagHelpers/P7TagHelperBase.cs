using Microsoft.AspNetCore.Razor.TagHelpers;

namespace P7.Core.TagHelpers
{
    public class P7TagHelperBase : TagHelper
    {
        private static string _version;

        public static string Version
        {
            get
            {
                if (string.IsNullOrEmpty(_version))
                {
                    _version = "not-set";
                }
                return _version;

            }
            set { _version = value; }
        }
        private static string _cbcPrepend;

        public static string CBVPrepend
        {
            get
            {
                if (string.IsNullOrEmpty(_cbcPrepend))
                {
                    _cbcPrepend = string.Format("/cb-v/{0}/", Version);
                }
                return _cbcPrepend;
            }
        }
    }
}