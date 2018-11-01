using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FunctionsCore.Modules;

using IdentityServer4.Endpoints.Results;

namespace GenericSecurityTokenService.Functions
{
    public class CoreIdentityFunction : IIdentityFunction
    {
        private IFunctionHttpContextAccessor _httpContextAccessor;

        public CoreIdentityFunction(IFunctionHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync()
        {
            if (_httpContextAccessor.HttpContext.User == null)
            {
                _httpContextAccessor.HttpResponseMessage.StatusCode = (HttpStatusCode.Unauthorized);
                return;
            }

            // Authentication boilerplate code end
            _httpContextAccessor.HttpResponseMessage.StatusCode = (HttpStatusCode.OK);
            var query = from claim in _httpContextAccessor.HttpContext.User.Claims
                let c = new {type = claim.Type, value = claim.Value}
                select c;

            _httpContextAccessor.HttpResponseMessage.Content = new JsonContent(query);

        }
    }
}
