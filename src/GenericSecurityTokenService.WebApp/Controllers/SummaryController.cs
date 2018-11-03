using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GenericSecurityTokenService.WebApp.Controllers
{
    [Route("")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private IActionContextAccessor _actionContextAccessor;
        public SummaryController(IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
        }
        private static Dictionary<string, object> _output;

        private static Dictionary<string, object> Output
        {
            get
            {
                if (_output == null)
                {
                    var credits = new Dictionary<string, string>()
                    {
                        {"IdentityServer4", "https://github.com/IdentityServer/IdentityServer4"},

                    };
                    _output = new Dictionary<string, object>
                    {
                        {"version", "1.0"},
                        {"application", "GenericSecurityTokenService"},
                        {"author", "Herb Stahl"},
                        {"credits", credits},

                    };
                }
                return _output;
            }
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IDictionary<string, object>> Get()
        {
            if (_output == null)
            {
                Output.Add("authority", _actionContextAccessor.ActionContext.HttpContext.GetIdentityServerHost());
            }
            return Output;
        }
    }

 
}
