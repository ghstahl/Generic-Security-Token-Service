using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;

namespace P7.Core.Middleware
{
    /// <summary>
    /// Extension methods for the <see cref="P7RewriteMiddleware"/>
    /// </summary>
    public static class RewriteBuilderExtensions
    {
        /// <summary>
        /// Checks if a given Url matches rules and conditions, and modifies the HttpContext on match.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        /// <returns></returns>
        public static IApplicationBuilder UseP7Rewriter(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<P7RewriteMiddleware>();
        }

        /// <summary>
        /// Checks if a given Url matches rules and conditions, and modifies the HttpContext on match.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        /// <param name="options">Options for rewrite.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseP7Rewriter(this IApplicationBuilder app, RewriteOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // put middleware in pipeline
            return app.UseMiddleware<P7RewriteMiddleware>(Options.Create(options));
        }
    }
}