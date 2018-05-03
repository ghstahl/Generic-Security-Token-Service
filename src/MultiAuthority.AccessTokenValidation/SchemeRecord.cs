using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MultiAuthority.AccessTokenValidation
{
    public class SchemeRecord
    {
        public string Name { get; set; }
        public Action<JwtBearerOptions> Options { get; set; }
    }
}