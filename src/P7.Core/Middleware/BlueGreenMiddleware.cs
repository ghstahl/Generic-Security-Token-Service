using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using P7.Core.Deployment;

namespace P7.Core.Middleware
{
    public class BlueGreenMiddleware
    {
        private readonly RequestDelegate _next;
        private IOptions<DeploymentOptions> _deploymentOptions;
        public BlueGreenMiddleware(RequestDelegate next,
            IOptions<DeploymentOptions> deploymentOptions)
        {
            _next = next;
            _deploymentOptions = deploymentOptions;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                bool bSlideit = context.Request.Cookies.ContainsKey($".bluegreen.{_deploymentOptions.Value.Color}");
                if (bSlideit)
                {

                    context.Response.Cookies.Append($".bluegreen.{_deploymentOptions.Value.Color}", "true",
                        new CookieOptions()
                        {
                            Expires = DateTime.Now.AddMinutes(40)
                        });
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
              
            }
            await _next(context);
        }
    }
}