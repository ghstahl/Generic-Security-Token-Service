using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace WizardAppApi.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"Hello", "There!"};
        }
    }
}