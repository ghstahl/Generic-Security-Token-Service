using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using P7.Core.Cache;

namespace GenericSecurityTokenService.Controllers
{
    [Route("")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private IActionContextAccessor _actionContextAccessor;
        private IConfiguration _configuration;
        private ILogger _logger;
        private ISingletonAutoObjectCache<SummaryController, Dictionary<string, object>> _objectCache;

        public SummaryController(
            IActionContextAccessor actionContextAccessor,
            ISingletonAutoObjectCache<SummaryController, Dictionary<string, object>> objectCache,
            IConfiguration configuration,
            ILogger<SummaryController> logger)
        {
            _actionContextAccessor = actionContextAccessor;
            _objectCache = objectCache;
            _configuration = configuration;
            _logger = logger;
        }

        Dictionary<string, object> GetOutput()
        {
            var dictionaryCache = _objectCache.Value;
            if (dictionaryCache.TryGetValue("summary-output", out var result))
            {
                return result as Dictionary<string, object>;
            }

            var credits = new Dictionary<string, string>()
            {
                {"IdentityServer4", "https://github.com/IdentityServer/IdentityServer4"},
            };
            var summary = new Dictionary<string, object>
            {
                {"TenantName",_configuration["TenantName"] },
                {"version", "1.0"},
                {"application", "GenericSecurityTokenService"},
                {"author", "Herb Stahl"},
                {"credits", credits},
                {"authority", _actionContextAccessor.ActionContext.HttpContext.GetIdentityServerOrigin() }
            };
            dictionaryCache["summary-output"] = summary;
            return summary;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IDictionary<string, object>>> GetAsync()
        {
            _logger.LogInformation("Summary Executing...");
            var output = GetOutput();
            return output;
        }
    }
}
