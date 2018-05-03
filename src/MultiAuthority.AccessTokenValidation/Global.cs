using System.Collections.Generic;
using MultiAuthority.AccessTokenValidation;

namespace Microsoft.AspNetCore.Builder
{
    internal static class Global
    {
        private static Dictionary<string, SchemeRecord> _schemeRecords;

        public static Dictionary<string, SchemeRecord> SchemeRecords => _schemeRecords ?? (_schemeRecords = new Dictionary<string, SchemeRecord>());
    }
}