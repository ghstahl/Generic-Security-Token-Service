using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace P7.Core.Middleware
{
    public class P7RewriteMiddleware
    {
        private RewriteMiddleware _originalRewriteMiddleware;

        public P7RewriteMiddleware(
            RequestDelegate next,
            IHostingEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory,
            IOptions<RewriteOptions> options)
        {
            _originalRewriteMiddleware = new RewriteMiddleware(next, hostingEnvironment, loggerFactory, options);
        }

        /// <summary>
        /// Executes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A task that represents the execution of this middleware.</returns>
        public new Task Invoke(HttpContext context)
        {
            var currentUrl = context.Request.Path + context.Request.QueryString;
            context.Items.Add("original-path", currentUrl);
            return _originalRewriteMiddleware.Invoke(context);
        }
    }
}