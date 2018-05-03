using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace apiHost.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("echo")]
        public string Echo(string message)
        {
            return message;
        }
        [HttpGet]
        [Authorize]
        [Route("echo2")]
        public string Echo2(string message)
        {
            return message;
        }
 
        [HttpGet]
        [Route("bind")]
        public string Bind(string subject)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, subject),
                new Claim("sub", subject),
                new Claim("aud", "yourdomain.com"),
                new Claim("aud", "nitro"),
            };
                

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Global.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



            var jwtToken = new JwtSecurityToken(
                issuer: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }
    }
}
