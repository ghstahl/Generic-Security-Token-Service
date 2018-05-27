using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace P7.Core.Deployment
{
    public class DeploymentOptions
    {
        public const string WellKnown_SectionName = "deployment";
        [JsonProperty("type")]
        public string Color { get; set; }
        public string Host { get; set; }
    }

    public static class DeploymentExtensions
    {
        public static IServiceCollection RegisterDeploymentConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeploymentOptions>(configuration.GetSection(DeploymentOptions.WellKnown_SectionName));
            return services;
        }

        public static void DropBlueGreenApplicationCookie(this HttpContext context,IOptions<DeploymentOptions> options)
        {
            context.Response.Cookies.Append($".bluegreen.{options.Value.Color}", "true",
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(40)
                });
        }
        public static void DeleteBlueGreenApplicationCookie(this HttpContext context, IOptions<DeploymentOptions> options)
        {
            context.Response.Cookies.Delete($".bluegreen.{options.Value.Color}");
        }
    }
}
