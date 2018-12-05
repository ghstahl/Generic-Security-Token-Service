using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;

namespace P7.Core.IRules
{
    public class RewriteLowerCaseRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;
            var host = request.Host;
            var pathBase = request.PathBase;
            var path = request.Path;
            if (host.HasValue)
            {
                if (host.Port == null)
                {
                    request.Host = new HostString(host.Host.ToLower());
                }
                else
                {
                    request.Host = new HostString(host.Host.ToLower(), (int)host.Port);
                }
            }

            if (pathBase.HasValue)
            {
                request.PathBase = new PathString(pathBase.Value.ToLower());
            }

            if (path.HasValue)
            {
                request.Path = new PathString(path.Value.ToLower());
                request.PathBase = new PathString(pathBase.Value.ToLower());
            }

            context.Result = RuleResult.ContinueRules;
        }
    }
}
