using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WizardAppApi.Services;

namespace WizardAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RemoteJsonFileController : ControllerBase
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IRemoteJsonFileLoader _remoteJsonFileLoader;

        public RemoteJsonFileController(IRemoteJsonFileLoader remoteJsonFileLoader, IHttpContextAccessor httpContextAccessor)
        {
            _remoteJsonFileLoader = remoteJsonFileLoader;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET api/values
        [HttpGet]
        [Route("open")]
        public async Task<ActionResult<object>> GetOpenJsonAsync(string file)
        {
            return await _remoteJsonFileLoader.LoadAsync(file);
        } // GET api/values
        [HttpGet]
        [Authorize("Daffy Duck")]
        [Route("closed")]
        public async Task<ActionResult<object>> GetClosedJsonAsync(string file)
        {
            var query = from item in _httpContextAccessor.HttpContext.User.Claims
                where item.Type == ClaimTypes.NameIdentifier
                select item;
            var nameClaim = query.FirstOrDefault();

            return await _remoteJsonFileLoader.LoadAsync($"users/{nameClaim.Value}/{file}");
        }
    }
}