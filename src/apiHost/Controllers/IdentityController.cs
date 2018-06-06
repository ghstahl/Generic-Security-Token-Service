using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace apiHost.Controllers
{
    public class ClaimHandle
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    [Route("api/[controller]")]
    [Authorize("aggregator_service")]
    public class IdentityController : Controller
    {
        // GET api/values
        [HttpGet]
        [Route("open")]
        [Authorize("aggregator_service.read_only")]
        public async Task<IEnumerable<ClaimHandle>> GetOpenAsync()
        {
            if (Request.HttpContext.User != null)
            {
                var query = from item in Request.HttpContext.User.Claims
                    let c = new ClaimHandle() {Name = item.Type, Value = item.Value}
                    select c;
                return query;
            }
            return null;
        }// GET api/values

        [HttpGet]
        [Route("closed")]
        [Authorize("aggregator_service.full_access")]
        public async Task<IEnumerable<ClaimHandle>> GetClosedAsync()
        {
            if (Request.HttpContext.User != null)
            {
                var query = from item in Request.HttpContext.User.Claims
                    let c = new ClaimHandle() { Name = item.Type, Value = item.Value }
                    select c;
                return query;
            }
            return null;
        }
    }
}