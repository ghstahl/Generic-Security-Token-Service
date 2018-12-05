using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using P7.Core.Cache;

namespace GlorifiedJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindController : ControllerBase
    {
        private ILogger<BindController> _logger;
        private IActionContextAccessor _actionContextAccessor;
        private ISingletonAutoObjectCache<BindController, Dictionary<string, object>> _objectCache;

        public BindController(
            IActionContextAccessor actionContextAccessor,
            ISingletonAutoObjectCache<BindController, Dictionary<string, object>> objectCache,
            ILogger<BindController> logger)
        {
            _actionContextAccessor = actionContextAccessor;
            _objectCache = objectCache;
            _logger = logger;
        }
        Dictionary<string, object> GetOutput()
        {

            var dictionaryCache = _objectCache.Value;

            if (dictionaryCache.TryGetValue("summary-output", out var result))
            {
                return result as Dictionary<string, object>;
            }

            var request = _actionContextAccessor.ActionContext.HttpContext.Request;

            var credits = new Dictionary<string, string>()
            {
                {
                    "ASP.NET Core Test Server",
                    "https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1"
                },

            };
            var summary = new Dictionary<string, object>
            {
                {"version", "1.0"},
                {"application", "AzureApiFunction"},
                {"author", "Herb Stahl"},
                {"credits", credits},
                {"authority", $"{request.Scheme}://{request.Host.Value}" }
            };

            dictionaryCache.TryAdd("summary-output", summary);
            return summary;
        }
        // GET api/values
        [HttpPost]
        public async Task<ActionResult<IDictionary<string, object>>>PostAsync()
        {
            _logger.LogInformation("Summary Executing...");
            var output = GetOutput();
            return output;
        }
    }
}