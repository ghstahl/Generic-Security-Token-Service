using System.Collections.Generic;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace GenericSecurityTokenService.Controllers
{
    [Route("")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private IActionContextAccessor _actionContextAccessor;
        private ILogger _logger;
        public SummaryController(IActionContextAccessor actionContextAccessor,ILogger<SummaryController> logger)
        {
            _actionContextAccessor = actionContextAccessor;
            _logger = logger;
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
            _logger.LogInformation("Summary Executing...");
            var host = _actionContextAccessor.ActionContext.HttpContext.Request.Host;
            _logger.LogInformation($"host.value:{host.Value} host.Host:{host.Host} host.HasValue:{host.HasValue} host.Port:{host.Port}");
            _logger.LogInformation(_actionContextAccessor.ActionContext.HttpContext.Request.Host.ToUriComponent());
            if (_output == null)
            {
                Output.Add("authority", _actionContextAccessor.ActionContext.HttpContext.GetIdentityServerHost());
            }
            return Output;
        }
    }

 
}
